using System.Collections.Generic;
using PGT.Core.Func;

namespace PGT.Core.DataStructures {

    public interface Edge<TVertex, TData> {
        TVertex u { get; }
        TVertex v { get; }
        int u_id { get; }
        int v_id { get; }
        TData data { get; }
    }

    public class SparseGraph<T, EdgeData> {

        struct SparseEdge : Edge<T, EdgeData> {
            public SparseEdge(SparseGraph<T, EdgeData> G, int u, int v){
                this.G = G;
                u_id = u;
                v_id = v;
                this.data = G.weight(u, v).Value();
            }
            SparseGraph<T, EdgeData> G;
            public int u_id { get; private set; }
            public int v_id { get; private set; }
            public T u { get { return G[u_id]; }}
            public T v { get { return G[v_id]; }}

            public EdgeData data { get; private set; }
        }

        T[] vertices;
        Sequence<T> v_seq;
        Dictionary<int, EdgeData>[] edges;
        Dictionary<T, int> v2i;

        public SparseGraph(Sequence<T> V, Sequence<Tuple<int, int, EdgeData>> E){
            vertices = V.ToArray();
            v_seq = Sequence.Array(vertices);
            
            v2i = new Dictionary<T, int>();
            
            var len = vertices.Length;
            for(int i=0; i<len; ++i){
                v2i[vertices[i]] = i;
            }

            edges = new Dictionary<int, EdgeData>[len];

            foreach(var edge in E){
                var u = edge.car;
                var v = edge.cdr;
                if (edges[u]==null) edges[u] = new Dictionary<int, EdgeData>();
                edges[u][v] = edge.cpr;
            }
        }

        public Sequence<int> Neighbours(int vertex) {
            return Sequence.UnrollN(edges[vertex].Keys);
        }

        public Sequence<int> NeighboursWhereV(int vertex, System.Func<int, bool> filter) {
            return Sequence.UnrollN(edges[vertex].Keys).Filter(filter);
        }
        
        public Sequence<int> NeighboursWhereE(int vertex, System.Func<EdgeData, bool> filter) {
            return Sequence.UnrollN(edges[vertex]).Filter(
                (KeyValuePair<int, EdgeData> i) => filter(i.Value)
            ).Map(
                (KeyValuePair<int, EdgeData> i) => i.Key        
            );
        }

        public int FanOut(int vertex){
            return edges[vertex].Count;
        }

        public Option<int> GetIndex(T vertex){
            if(v2i == null || !v2i.ContainsKey(vertex)) return Option.None<int>();
            return Option.Some(v2i[vertex]);    
        }


        public Option<EdgeData> weight(int u, int v){
            if(!edges[u].ContainsKey(v)) return Option.None<EdgeData>();
            return Option.Some(edges[u][v]);
        }
        
        public T this[int index]{
            get {
                return vertices[index];
            }
        }

        public Edge<T, EdgeData> this[int u, int v] {
            get {
                return new SparseEdge(this, u, v);
            }
        }
    }

}