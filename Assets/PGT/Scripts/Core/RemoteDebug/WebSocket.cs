#if NETFX_CORE

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using PGT.Core;

namespace PGT.Core.RemoteDebug {

	public class WebSocket : IDisposable {

		MessageWebSocket ws;
		public volatile bool connected = false;
		Uri uri;
		DataWriter writer = null;

		Action<string> responseParser;

		public WebSocket(string url, Action<string> responseParser) {
			ws = new MessageWebSocket();
			ws.Control.MessageType = SocketMessageType.Utf8;
			ws.MessageReceived += OnMessageReceived;
			uri = new Uri(url);
			this.responseParser = responseParser;
			Task.Run(() => Connect());
		}

		public async void Connect(){
			await ws.ConnectAsync(uri);
			writer = new DataWriter(ws.OutputStream);
			connected = true;
		}

		public void SendMessage(string message){
			Task.Run(() => _sendMessage(message));
		}

		async void _sendMessage(string message){
			writer.WriteString(message);
			await writer.StoreAsync();
		}

		void OnMessageReceived(MessageWebSocket sender, MessageWebSocketMessageReceivedEventArgs args){
			using (DataReader reader = args.GetDataReader()){
				responseParser?.Invoke(reader.ReadString(reader.UnconsumedBufferLength));
			}
		}

        public override string ToString(){
            return "WebSocket Instance at "+uri.ToString()+", connected="+(connected ? "true" : "false");
        }

		public void Dispose(){
			ws.Dispose();
		}
	}
}

#endif