from neural_network import *
from train_model import *

if __name__ == '__main__':
    net = NeuralNetwork()
    net.add_layer(Dense(2, 16))
    net.add_layer(Activation(tanh, tanh_prime))
    net.add_layer(Dense(16, 4))
    net.add_layer(Activation(sigmoid, sigmoid_prime))

    x, y = read_points_data()
    train_network(x, y, net)

    for point in x:
        point = np.reshape(point, (2, 1))
        print(net.feed_forward(point))

    # net = NeuralNetwork()
    # net.add_layer(Dense(2, 8))
    # net.add_layer(Activation(tanh, tanh_prime))
    # net.add_layer(Dense(8, 1))
    # net.add_layer(Activation(sigmoid, sigmoid_prime))
    #
    # x, y = get_xor_data()
    # train_network(x, y, net)
