using System.Collections.Generic;
using UnityEngine;

public class QuaternionDamper {

    LinkedList<Quaternion> buffer;
    int capacity;
    public QuaternionDamper(int capacity){
        this.capacity = capacity;
        buffer = new LinkedList<Quaternion>();
    }

    public void Update(Quaternion value){
        buffer.AddLast(value);
        while(buffer.Count > capacity){
            buffer.RemoveFirst();
        }
    }

    public Quaternion Value() {
        var count = 0;
        if (buffer.Count == 0) return Quaternion.identity;
        var average = buffer.First.Value;

        foreach(var item in buffer){
            var frac = 1f / ++count;
            average = Quaternion.Slerp(item, average, frac);
        }

        return average;
    }

}