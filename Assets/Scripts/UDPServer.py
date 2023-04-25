# Server imports
from socket import *
from threading import Thread
import queue

# Leap imports
import os
import numpy as np
from toolz import pipe

from leap_ec.individual import Individual
from leap_ec.decoder import IdentityDecoder
from leap_ec import context, probe

import leap_ec.ops as ops
from leap_ec.binary_rep.problems import MaxOnes
from leap_ec.binary_rep.initializers import create_binary_sequence
from leap_ec.binary_rep.ops import mutate_bitflip
from leap_ec import util
from leap_ec.binary_rep.problems import ScalarProblem
import argparse
import sys

# Set up the server
serverSocket = socket(AF_INET, SOCK_DGRAM) #
serverSocket.bind(('localhost', 5500))
print("Server ready.")

fitnesses = queue.Queue()

# Define the problem
class EvolveStats(ScalarProblem):
    def __init__(self):
        super().__init__(maximize=True)

    def evaluate(self, ind):
        if(fitnesses.empty()):
            f = "0"
        else:
            f = fitnesses.get()

        print(f + " (f)")
        return float(f)

# Send the current gen to the game
def getGen(gen, address):
    ret = ""
    for g in gen:
        trait = g.genome.tolist()
        trait = ''.join(str(digit) for digit in trait)
        trait = " ".join(trait[i:i+8] for i in range(0, len(trait), 8))
        trait += " "
        ret += trait
    print(ret)

    serverSocket.sendto(ret.encode(), address)

def Evolve(max_generation=5, p_m=.01, p_c=.3, trn_size=2, csv_output="output.csv"):
    print("Max_gen: " + str(max_generation) + " p_m: " + str(p_m) + " p_c: " + str(p_c) + " trn_size: " + str(trn_size))
    # Evaluate initial population
    pop_size = 10
    parents = Individual.create_population(pop_size,
                                           initialize=create_binary_sequence(24),
                                           decoder=IdentityDecoder(),
                                           problem=EvolveStats())


    parents = Individual.evaluate_population(parents)

    message, address = serverSocket.recvfrom(1024)
    message = message.decode()
    print(message)
    getGen(parents, address)
    fmessage, address = serverSocket.recvfrom(1024)
    fitness = fmessage.decode()
    print("Fitness is: " + fitness)
    fits = fitness.split(" ")
    for i in range(pop_size):
        fitnesses.put(fits[i])



    generation_counter = util.inc_generation()
    out_f = open(csv_output, "w")
    num = 0
    while generation_counter.generation() < max_generation:
        print(num)
        num += 1
        message, address = serverSocket.recvfrom(1024)
        message = message.decode()

        offspring = pipe(parents,
                         ops.tournament_selection(k=trn_size),
                         ops.clone,
                         mutate_bitflip(probability=p_m),
                         ops.uniform_crossover(p_xover=p_c),
                         ops.evaluate,
                         ops.pool(size=len(parents)),  # accumulate offspring
                         probe.AttributesCSVProbe(stream=out_f, do_fitness=True, do_genome=True)
                        )

        parents = offspring
        getGen(parents, address)
        fmessage, address = serverSocket.recvfrom(1024)
        fitness = fmessage.decode()
        print("Fitness is: " + fitness)
        fits = fitness.split(" ")
        for i in range(pop_size):
            fitnesses.put(fits[i])

        generation_counter()  # increment to the next generation

    offspring = pipe(parents,
                     ops.tournament_selection(k=trn_size),
                     ops.clone,
                     mutate_bitflip(probability=p_m),
                     ops.uniform_crossover(p_xover=p_c),
                     ops.evaluate,
                     ops.pool(size=len(parents)),  # accumulate offspring
                     probe.AttributesCSVProbe(stream=out_f, do_fitness=True, do_genome=True)
                    )

    out_f.close()
    print("Game Over!")

print("Getting settings:")
message, address = serverSocket.recvfrom(1024)
max_gen = message.decode()
message, address = serverSocket.recvfrom(1024)
p_m = message.decode()
message, address = serverSocket.recvfrom(1024)
p_c = message.decode()
message, address = serverSocket.recvfrom(1024)
tourn = message.decode()

Evolve(int(max_gen), float(p_m), float(p_c), int(tourn))
