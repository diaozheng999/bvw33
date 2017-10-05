using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PGT.Core;
using PGT.Core.Func;
using PGT.Core.DataStructures;


public struct Pose {
	public float[] coefficient;
	public float intercept;

	Sequence<float> coefficient_seq;

	public float Compute(Sequence<float> features){
		coefficient_seq = coefficient_seq ?? Sequence.Array(coefficient);
		var exponent = coefficient_seq.MapWith(features, Function.fmul).Reduce(Function.fadd, intercept);
		var h0 = 1 / (1 + Mathf.Exp(-exponent));
        return h0;
	}
}