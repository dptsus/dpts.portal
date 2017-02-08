using System;
using System.IO;
using System.Net;

namespace DPTS.NotifyWinService
{
    public class HealthCheck
    {
        public static readonly bool IsSystemHealthy = true;

        HttpListener _httpListener = null;
      
        public void OpenConnection()
        {
            if (_httpListener != null)
                throw new InvalidOperationException("Http listener is already running.");
            _httpListener = new HttpListener();
            _httpListener.Prefixes.Add("http://+:8734/health/");
            _httpListener.Start();
            _httpListener.BeginGetContext(OnGetContextAsync, _httpListener);    
        }

        private async void OnGetContextAsync(IAsyncResult ar)
        {
            var listener = ar.AsyncState as HttpListener;
            var context = listener?.EndGetContext(ar);
            var stream = context?.Response.OutputStream;

           
            _httpListener.BeginGetContext(OnGetContextAsync, listener);
        }
        
        private void OnWrite(IAsyncResult ar)
        {
            try
            {
                var stream = ar.AsyncState as Stream;
                stream?.EndWrite(ar);
                stream?.Close();
            }
            catch
            {
                //ignore
            }
        }

        public void CloseConnection()
        {
            _httpListener?.Stop();
            _httpListener = null;
        }
    }
}
