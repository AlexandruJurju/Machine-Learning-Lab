import csv
from typing import Tuple

from neural_network import *


def read_points_data() -> Tuple:
    file = open("points_train.txt")
    split = file.read().split("\n")

    x = []
    y = []
    zones = 0

    for i, point in enumerate(split):
        values = point.split(" ")
        if i == 0:
            zones = int(values[0])
        else:
            x_coord = int(values[0]) / 300
            y_coord = int(values[1]) / 300
            target_zone = int(values[2])

            x.append([x_coord, y_coord])

            output_list = []
            for zone in range(zones):
                if zone == target_zone:
                    output_list.append(1)
                else:
                    output_list.append(0)
            y.append(output_list)

    return x, y


def get_xor_data() -> Tuple:
    x = [[0, 0], [0, 1], [1, 0], [1, 1]]
    y = [[0], [1], [1], [0]]

    return x, y


def train_network(x, y, network: NeuralNetwork):
    # reshape length of x, number of inputs per line,
    x = np.reshape(x, (np.shape(x)[0], np.shape(x)[1], 1))
    y = np.reshape(y, (np.shape(y)[0], np.shape(y)[1], 1))

    network.train(mse, mse_prime, x, y, 0.1)

    wrong_count = 0
    for x_test, y_test in zip(x, y):
        output = network.feed_forward(x_test)
        print(np.shape(x))

        output_index = list(output).index(max(list(output)))
        target_index = list(y_test).index(max(list(y_test)))
        print(f"target = {target_index}, output = {output_index}")
        print("============================================")

        if target_index != output_index:
            wrong_count += 1

    print(f"{wrong_count / len(x)}")
