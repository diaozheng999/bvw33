using System;
using System.IO;
using PGT.Core.DataStructures;


public class CSVWriter : IDisposable {

	FileStream writer;

	public CSVWriter(string filepath) {
		writer = File.OpenWrite(filepath);
	}

	public void Write<T>(Sequence<T> f, Func<T, string> to_string_fn){
		var str_seq = f.Map(to_string_fn).ToArray();
		var to_write = string.Join(",", str_seq)+"\n";
		var bytes = System.Text.ASCIIEncoding.ASCII.GetBytes(to_write);
		writer.Write(bytes, 0, bytes.Length);
	}

	public void Dispose(){
		writer?.Close();
		writer?.Dispose();
		writer = null;
	}

}
