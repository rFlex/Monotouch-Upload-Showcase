using System;
using System.Net;
using System.IO;
using System.Net.Cache;

namespace TestUpload
{
	public class Uploader : IDisposable {

		////////////////////////////////////
		// VARIABLES
		////////////////////

		public event Action<Uploader> OnProgressChanged;
		public string Uri { get; set; }
		public byte[] DataToUpload { get; set; }
		public float UploadedRatio { get; private set; }
		private bool disposed;

		////////////////////////////////////
		// CONSTRUCTORS
		////////////////////

		public Uploader (string uri = null) {
			this.Uri = uri;
		}


		////////////////////////////////////
		// METHODS
		////////////////////

		public void Dispose() {
			this.disposed = true;
		}

		public string Upload() {
			Console.WriteLine ("Starting upload");
			if (this.DataToUpload == null) {
				throw new Exception ("No Data set in the Uploader");
			}
			if (this.Uri == null) {
				throw new Exception ("No URI set in the Uploader");
			}
			string responseStr = null;
			HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(this.Uri);
			request.Method = "POST";
			request.ContentType = "image/jpeg";
			request.AllowWriteStreamBuffering = false;
			request.CachePolicy = null;
			request.ContentLength = this.DataToUpload.Length;

			using (Stream requestStream = request.GetRequestStream ()) {
				int uploadedSoFar = 0;

				while (uploadedSoFar < this.DataToUpload.Length) {
					if (this.disposed) {
						throw new Exception ("Canceled");
					}

					int toUpload = 8192;
					if (uploadedSoFar + toUpload > this.DataToUpload.Length) {
						toUpload = this.DataToUpload.Length - uploadedSoFar;
					}

					requestStream.Write (this.DataToUpload, uploadedSoFar, toUpload);
					uploadedSoFar += toUpload;

					this.SetUploadedRatio ((float)uploadedSoFar / (float)this.DataToUpload.Length);
				}

				Console.WriteLine ("Will get response");

				WebResponse response = request.GetResponse ();

				Console.WriteLine ("Getting response");
				using (Stream responseStream = response.GetResponseStream()) {
					using (StreamReader reader = new StreamReader(responseStream)) {
						responseStr = reader.ReadToEnd ();
					}
				}
			}

			this.SetUploadedRatio (1);

			return responseStr;
		}

		public delegate void ResponseHandler (string response, Exception thrownException);
		public void UploadAsync(ResponseHandler responseHandler) {
			Action upload = delegate {
				string result = null;
				Exception thrownException = null;

				try {
					result = this.Upload();
				} catch (Exception e) {
					thrownException = e;
				}
				if (responseHandler != null) {
					responseHandler(result, thrownException);
				}
			};

			upload.BeginInvoke (null, null);
		}

		public void SetUploadedRatio(float uploadedRatio) {
			this.UploadedRatio = uploadedRatio;

			if (this.OnProgressChanged != null) {
				this.OnProgressChanged (this);
			}
		}

		////////////////////////////////////
		// GETTERS/SETTERS
		////////////////////

	}
}

