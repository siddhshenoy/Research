import UnityEngine
import os
from sklearn.neural_network import MLPClassifier
from sklearn.preprocessing import OneHotEncoder
from sklearn.linear_model import LogisticRegression
import pickle
import numpy as np

read_file = open("Assets/params.out", "r")
data = read_file.read()
read_file.close()

float_arr = []
data_split = data.split(" ")
for i in range(0, len(data_split)):
    float_arr.append(float(data_split[i]))
float_arr = np.array([float_arr])
clf = pickle.load(open("Assets/mlp_weights.pkl", "rb"))
result = clf.predict(float_arr)
UnityEngine.Debug.Log(float(result))
