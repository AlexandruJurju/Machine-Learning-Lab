from neural_network import *
from train_model import *

if __name__ == '__main__':
    # Points
    net = NeuralNetwork()
    net.add_layer(Dense(2, 16))
    net.add_layer(Activation(tanh, tanh_prime))
    net.add_layer(Dense(16, 4))
    net.add_layer(Activation(sigmoid, sigmoid_prime))

    x, y = read_points_data()
    train_point_network(x, y, net, 0.5)

    # print(net.feed_forward(np.reshape((176 / 300, -142 / 300), (2, 1))))

    # XOR
    # netXOR = NeuralNetwork()
    # netXOR.add_layer(Dense(2, 8))
    # netXOR.add_layer(Activation(tanh, tanh_prime))
    # netXOR.add_layer(Dense(8, 1))
    # netXOR.add_layer(Activation(sigmoid, sigmoid_prime))
    #
    # xXOR, yXOR = get_xor_data()
    # train_xor(xXOR, yXOR, netXOR)
