namespace Common.FastDFS
{
    internal interface IFDFSResponse
    {
        void ParseBuffer(byte[] responseBytes, int length);
    }

    internal class EmptyFDFSResponse: IFDFSResponse
    {
        public void ParseBuffer(byte[] responseBytes, int length)
        {
          
        }
    }
}
