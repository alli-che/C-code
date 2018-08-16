import pandas as pd
import numpy as np
import csv
import math
import nltk
import scipy as sp
from scipy import signal
from scipy.signal import butter, lfilter, freqz, firwin,kaiserord
import matplotlib.pyplot as plt
import re
import os
import sys

''' this is the most corrected version of code!!!!!!!!!!!!!! 7/31'''

def firfilt(data):
    nfreq = 0.04
    taps =  6
    a = 1
    b = signal.firwin(taps, cutoff=nfreq)
    firstpass = signal.lfilter(b, a, data)
    ## second pass to compensate phase delay
    secondpass = signal.lfilter(b, a, firstpass[::-1])[::-1]
    return secondpass

def openFiles(myRawFile):
    #cwd = os.getcwd()
    #cwd += "/temp.txt"
    new_file = open("temp.txt", "w")

    # ANDY P ALL UP IN THIS CODE YOOOOOOOO

    file = open(myRawFile,"r")

    for j, l in enumerate(file):
        pass

    file.close()
    file = open(myRawFile,"r")

    for k in range(j+1):
        row = file.readline()
        newrow = row.replace("[", "")
        newrow = newrow.replace("'","")
        newrow = newrow.replace(" ", "")
        newrow = newrow.replace("(", "")
        newrow = newrow.replace(")", "")
        newrow = newrow.replace("]", "")
        new_file.write(newrow)

    file.close()

    new_file.close()
    dataTXT = open("temp.txt", "r")

    return dataTXT

def fixFormat(dataTXT):
    fixedData = pd.DataFrame()
    fixedData = pd.read_table(dataTXT, sep=',', header=None)

    fixedSize = fixedData.shape
    fixedData.drop(fixedData.columns[fixedSize[1]-1],axis=1, inplace=True)

    dataTXT.close()
    df = pd.DataFrame()
    df = df.append(fixedData.loc[:,0], ignore_index = True)
    dfshap = df.shape

    dataTXT1 = open("time.txt", "w")
    for i in range(dfshap[1]):
        k = format(df.iloc[0,i],'.3f')
        newStr = k[0:2] + "," + k[2:4] + "," + k[4:6] + "," + k[7:] +"\n"
        dataTXT1.write(newStr)

    dataTXT1.close()
    correctTime = pd.read_csv("time.txt", header=None)
    return correctTime, fixedData

def timeTest(dfTime):
    time_dataMoCap = dfTime
    dimM = time_dataMoCap.shape
    sizeM = dimM[0]
    timeListM = []
    for i in range(sizeM):
            a = time_dataMoCap.iloc[i,0]
            minM = a * (3600)
            b = time_dataMoCap.iloc[i,1]
            secM = b * (60)
            cM = time_dataMoCap.iloc[i,2]
            d = time_dataMoCap.iloc[i,3]
            nanoM = d/1000
            calc = minM+secM+cM+nanoM
            timeListM +=[calc]


    column2M = ["time"]
    calc_timeM = pd.DataFrame(np.array(timeListM), columns = column2M)
    val1M = (time_dataMoCap.iloc[sizeM-1,0] - time_dataMoCap.iloc[0,0]) *(3600)
    val2M = (time_dataMoCap.iloc[sizeM-1,1] - time_dataMoCap.iloc[0,1]) *(60)
    val3M =  (time_dataMoCap.iloc[sizeM-1,2] - time_dataMoCap.iloc[0,2])
    val4M =  (time_dataMoCap.iloc[sizeM-1,3] - time_dataMoCap.iloc[0,3]) / 1000
    totalValM = val1M + val2M + val3M + val4M
    # total time of the study
    mDuration = int(math.floor(totalValM))

    os.remove("temp.txt")
    os.remove("time.txt")
    return mDuration,calc_timeM

def zeroingData(calc_timeM):
    # this uses dataframe 2/calc_timeM for the calculations
    # getting a zeroed out dataframe
    zeroingM = []
    timeSize = calc_timeM.shape
    for i in range(timeSize[0]):
        zero = abs(calc_timeM.iloc[i,0]-calc_timeM.iloc[0,0])
        zeroingM += [zero]

    # creating new dataframe with zeroed data
    column3M = ["seconds"]
    zeroed_dataM = pd.DataFrame(np.array(zeroingM), columns = column3M)

    return zeroed_dataM, timeSize[0]

def secondsStudy(zeroed_dataM, mDur, sizeT):
# creating dataframe for every second of the study
    fxxM = []
    for k in range(sizeT):
        fx = []
        for j in range(mDur):
            f = abs(j - zeroed_dataM.iloc[k,0])
            fx += [f]
        fxxM += [fx]

    xM = np.array(fxxM)
    study_dataM = pd.DataFrame(np.array(fxxM))
    return study_dataM

def findingMIN(secondsdf, mDur):
    #finding the minimum of each column

    minsM = []

    for i in range(mDur):
        minsM += [secondsdf[i].idxmin(axis=1)]

    return minsM

def filtering(fData):
    mocap_df = pd.DataFrame()
    # convert data to data frame
    #should change this to i loc
    mocap_df = mocap_df.append(fData.loc[:,1:], ignore_index = True)
    mosh = mocap_df.shape
    moshDuration = mosh[1]
    filteredDF = pd.DataFrame()
    i = 1
    while i in range(moshDuration+1):
        interSD = mocap_df.loc[:,i]
        arr = np.array(interSD.values)
        y = firfilt(arr)
        sample = pd.DataFrame(np.array(y))
        filteredDF = filteredDF.append(sample.T, ignore_index = True)
        i+=1
    filtDF = filteredDF.T
    return filtDF, moshDuration


def upsample(filtdf,mDur,minL,moD,newCSV,dest):

    newCSV +=".csv"
    path = dest + newCSV
    print(newCSV)
    # calculate upsample
    upsampyM = pd.DataFrame()
    for j in range(mDur-1):
        x = []
        interDFM = filtdf.loc[minL[j]:minL[j+1], :]

        # for each column
        for i in range(moD):
            # conver to np array
            arr = np.array(interDFM.loc[:,i].values)
            # upsample
            z = signal.resample(arr, 50)
            #80
            #z = arr
            #z = signal.resample_poly(arr, 50, 75)
            # store in array of arrays
            x += [z]

            # convert array of arrays to
            sample = pd.DataFrame(np.array(x))

        upsampyM = upsampyM.append(sample.T, ignore_index = True)

    upsampyM.to_csv(path, sep=',',header=False,index=False)

    return upsampyM



if __name__ == "__main__":
    print("Python code executing...\n")
    pwd = os.getcwd()
    print(pwd)
    desination = sys.argv[1]
    cwd = sys.argv[2]
    os.chdir(cwd)
    print(cwd)
    files = os.listdir()
    print(files)
    csvs = []
    rawFiles = []
    #desination = "/Users/alli/Desktop/VisualStudioForUnity/PythonCallFromUnity/PythonCall/upSampledBody/"
    for fi in files:
        if fi.endswith(".txt"):
            rawFiles+=[fi]
            st = fi.split(".")
            csvs +=[st[0]]

    print(rawFiles)
    i = 0
    for rawF in rawFiles:
        csvs[i]
        #cwd += "/temp.txt"
        #pwd += "/temp.txt"

        #dataText = openFiles(cwd, rawF)
        dataText = openFiles(rawF)
        timeFrame, fixedFrame = fixFormat(dataText)
        totalStudyTime, totalTimedf =  timeTest(timeFrame)
        zeroed_dataF, dataLength = zeroingData(totalTimedf)
        secondsDF = secondsStudy(zeroed_dataF, totalStudyTime, dataLength)
        minList = findingMIN(secondsDF, totalStudyTime)
        filteredData, moshD = filtering(fixedFrame)
        upsampled = upsample(filteredData, totalStudyTime,minList,moshD,csvs[i],desination)
        #print(upsampled.shape)
        i+=1

    #print("done in the python code")
