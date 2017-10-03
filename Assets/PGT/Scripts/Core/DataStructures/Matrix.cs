using PGT.Core.Func;
using System;
using UnityEngine;

namespace PGT.Core.DataStructures
{

    public class Matrix
    {
        private Sequence<float> values;

        private int m;
        private int n;

        public Matrix(int m, int n, Func<int, int, float> f)
        {
            values = Sequence.Tabulate(m * n, (int i) => f(i / n, i % n));
            this.m = m;
            this.n = n;
        }

        public Matrix(int m, int n, Sequence<float> f) {
            values = f;
            this.m = m;
            this.n = n;
        }


        public float this[int i, int j]
        {
            get
            {
                return values[i * n + j];
            }
            set
            {
                values[i * n + j] = value;
            }
        }

        public Sequence<float> this[int i]
        {
            get
            {
                return values.Subseq(i * n, n);
            }
        }

        public Sequence<float> col(int j)
        {
            return values.FilterIndex((int i) => i % n == j);
        }

        float _transpose_fn(int i, int j) {
            return this[j, i];
        }

        public Matrix transpose()
        {
            return new Matrix(n, m, _transpose_fn);
        }

        public Matrix Resize(int m, int n){
            return new Matrix(m, n, values);
        }

        public Matrix Inverse() {
            if (this.m != this.n) throw new ArithmeticException("Dimension Mismatch.");
            var dim = this.n;

            // Using gaussian elimination here. We could conceivably use
            // LU decomposition or SVD or Block inversion to find such
            // an inverse, but since we're dealing with a small matrix
            // and inverting it once, I don't really care.


            // build identity matrix
            var inv = new float[dim * dim];
            // here, we're abusing the fact that Seq.Array doesn't create a copy
            // of said array and just *assumes* that the underlying array is
            // immutable. This means we can use the sequence operations on these
            // arrays.
            var inv_seq = Sequence.Array(inv); 
            for(int i=0; i<dim; ++i) inv[i*dim+i] = 1;

            var mat = values.ToArray();
            // Abusing immutability assumption here too :)
            var mat_seq = Sequence.Array(mat);

            for(int i=0; i<dim; ++i){
                var row_start = i * dim;
                var value = mat[row_start + i];

                if(Mathf.Abs(value) < Mathf.Epsilon){
                    // Value smaller than machine epsilon, swapping with the next
                    // available column
                    var j = mat_seq.MapIndex((int k, float v) => {
                        var k_dim = k % dim;
                        if (k_dim == i && (Mathf.Abs(value) > Mathf.Epsilon)) {
                            return k_dim;
                        }
                        return int.MaxValue;
                    }).Reduce(Mathf.Min, int.MaxValue);

                    if (j == int.MaxValue) throw new ArithmeticException("Singular Matrix");

                    var alt_start = j * dim;

                    for(int k=0; k<dim; ++k){
                        var buf = mat[row_start+k];
                        mat[row_start+k] = mat[alt_start+k];
                        mat[alt_start+k] = buf;
                    }
                    for(int k=0; k<dim; ++k){
                        var buf = inv[row_start+k];
                        inv[row_start+k] = inv[alt_start+k];
                        inv[alt_start+k] = buf;
                    }

                    value = mat[row_start + i];
                }

                // normalise the identity element
                mat_seq.Subseq(row_start, dim).Map((float v) => v / value).CopyTo(mat, row_start);
                inv_seq.Subseq(row_start, dim).Map((float v) => v / value).CopyTo(inv, row_start);

                // subtract this row from the other rows
                for(int j=0; j<dim; ++j){
                    if(j==i) continue;
                    var j_row_start = j * dim;
                    var normalise = mat[j_row_start + i];
                    if(Mathf.Abs(normalise) < Mathf.Epsilon) continue;

                    mat_seq.Subseq(j_row_start, dim).MapWith(mat_seq.Subseq(row_start, dim),
                        (float j_row, float i_row) => j_row - normalise * i_row
                    ).CopyTo(mat, j_row_start);
                    inv_seq.Subseq(j_row_start, dim).MapWith(inv_seq.Subseq(row_start, dim),
                        (float j_row, float i_row) => j_row - normalise * i_row
                    ).CopyTo(inv, j_row_start);
                }
            }

            return new Matrix(dim, dim, inv_seq);
        }

        public override string ToString(){
            var str = "";
            for(int i=0; i<this.m; i++){
                str += "["+String.Join(", ", this[i].Map((float v) => v.ToString()).ToArray())+"]\n";
            }
            return str;
        }

        public static Matrix operator *(Matrix x, Matrix y)
        {
            if (x.n != y.m) throw new ArithmeticException("Dimension Mismatch.");

            return new Matrix(x.m, y.n, (int i, int j) => x[i].MapWith(y.col(j), Function.fmul).Reduce(Function.fadd, 0f));
        }

        public static Vector3 operator *(Matrix x, Vector3 y){
            if(x.n < 3 || x.n > 4) throw new ArithmeticException("Dimension Mismatch.");
            var y_mat = new Matrix(x.n, 1, Sequence.OfVector3OneExtended(y, x.n));
            var result_mat = x * y_mat;
            Debug.Log(result_mat);
            var result = new Vector3(
                result_mat[0, 0],
                result_mat[1, 0],
                result_mat[2, 0]
            );
            if (x.m == 4) result /= result_mat[3, 0];
            return result;
        }

        public static Vector3 operator *(Vector3 x, Matrix y) => y.transpose() * x;


        public static Matrix Eye(int dim)
        {
            return new Matrix(dim, dim, (int i, int j) => i==j ? 1 : 0);
        }

        public static Matrix Column(Vector3 vec){
            return new Matrix(3, 1, (int i, int j) => vec[i]);
        }

        public static Matrix Row(Vector3 vec){
            return new Matrix(1, 3, (int i, int j) => vec[j]);
        }

        public static Matrix ColStack(Vector3[] vec){
            return new Matrix(3, vec.Length, (int i, int j) => vec[j][i]);
        }

        public static Matrix RowStack(Vector3[] vec){
            return new Matrix(vec.Length, 3, (int i, int j) => vec[i][j]);
        }
    }

}