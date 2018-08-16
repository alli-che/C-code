import pandas as pd
import numpy as np
import csv
import math
import nltk
import scipy as sp
from scipy import signal
from scipy.signal import butter, lfilter, freqz, firwin,kaiserord
import matplotlib.pyplot as plt
import sys
import os

def FNIRS(destination):
    # name the columns of the time data frame
    columns = ["min","sec","ms","ns"]
    # read from time.csv and name the columns
    time_datafNIRS = pd.read_csv("time.csv", names=columns)
    # transpose the data for calculations
    time_datafNIRS.T
    # get the size of the data frame
    dim = time_datafNIRS.shape
    # actual size of the data
    size = dim[0]
    # calculated time
    timeList = []
    for i in range(size):
            a = time_datafNIRS.iloc[i,0]
            min = a * (3600)
            b = time_datafNIRS.iloc[i,1]
            sec = b * (60)
            c = time_datafNIRS.iloc[i,2]
            d = time_datafNIRS.iloc[i,3]
            nano = d/1000
            calc = min+sec+c+nano
            timeList +=[calc]
    # name the column for new dataframe and create it from a list
    column2 = ["time"]
    calc_time = pd.DataFrame(np.array(timeList), columns = column2)
    #print(df2)
    # creating fduration
    #perform individual calculations percolumn
    val1 = (time_datafNIRS.iloc[size-1,0] - time_datafNIRS.iloc[0,0]) *(3600)
    val2 = (time_datafNIRS.iloc[size-1,1] - time_datafNIRS.iloc[0,1]) *(60)
    val3 =  (time_datafNIRS.iloc[size-1,2] - time_datafNIRS.iloc[0,2])
    val4 =  (time_datafNIRS.iloc[size-1,3] - time_datafNIRS.iloc[0,3]) / 1000
    totalVal = val1 + val2 + val3 + val4
    # total time it took for study to complete
    fDuration = int(math.floor(totalVal))
    # this uses dataframe 2 for the calculations
    # getting a zeroed out dataframe
    zeroing = []
    for i in range(size):
        zero = calc_time.iloc[i,0]-calc_time.iloc[0,0]
        #print(zero)
        zeroing += [zero]
    #create zeroed data frame
    column3 = ["seconds"]
    zeroed_data = pd.DataFrame(np.array(zeroing), columns = column3)
    # every second of the data is calculated
    fxx = []
    for k in range(size):
        fx = []
        for j in range(fDuration):
            f = abs(j - zeroed_data.iloc[k,0])
            fx += [f]
        fxx += [fx]
    # turn multidimentional list into dataframe
    x = np.array(fxx)
    study_data = pd.DataFrame(np.array(fxx))
    #clear row of unnessecary information
    study_data.drop(study_data.index[0], inplace=True)
    # gather minimums of each columns for upsampling
    mins = []
    for i in range(fDuration):
        if i == 0:
            mins += [study_data[i].idxmin()]
        else:
            mins += [study_data[i].idxmin() + 1]
    oxy = np.genfromtxt('oxy.csv', delimiter=',')
    oxy_df = pd.DataFrame(np.array(oxy))
    oxysh = oxy_df.shape
    oxyDuration = oxysh[1]
    upsampy = pd.DataFrame()
    for j in range(fDuration-1):
        x = []
        for i in range(oxyDuration):
            interDF = oxy_df.loc[mins[j]-1:mins[j+1]-1, :]
            arr = np.array(interDF.loc[:,i].values)
            z = signal.resample(arr, 50)
            x += [z]
        sample = pd.DataFrame(np.array(x))
        upsampy = upsampy.append(sample.T, ignore_index = True)

        altered = upsampy.T

    destination += "\\fnirs.csv"
    altered.to_csv(destination, sep=',',header=False,index=False)

if __name__ == "__main__":
    print("Python code executing...\n")
    pwd = os.getcwd()
    print(pwd)
    dest = sys.argv[1]
    cwd = sys.argv[2]
    os.chdir(cwd)
    print(cwd)
    files = os.listdir()
    print(files)

    FNIRS(dest)
