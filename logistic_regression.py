import pandas as pd
import json
import io
from sklearn.linear_model import LogisticRegression

data1 = pd.read_csv("log3.csv", header=None)
data2 = pd.read_csv("log2.csv", header=None)
data3 = pd.read_csv("log1.csv", header=None)

data4 = pd.read_csv("log_more.csv", header=None)

x1 = data1[range(100)]
y1 = data1[100]
x2 = data2[range(100)]
y2 = data2[100]
x3 = data3[range(100)]
y3 = data3[100]

x4 = data3[range(100)]
y4 = data3[100]


model = LogisticRegression()



model.fit(x4, y4)
model.fit(x3, y3)   
model.fit(x2, y2)

_d  = {
    'coefficient' : model.coef_.tolist()[0],
    'intercept' : model.intercept_[0]
}

with file('pose1.json', 'w') as f:
  f.write(json.dumps(_d))


def accuracy(y_true, y_pred):
    return (y_true==y_pred).sum() / float(len(y_true))