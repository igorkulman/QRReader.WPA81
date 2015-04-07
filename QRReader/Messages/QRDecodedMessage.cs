namespace QRReader.Messages
{
    public class QRDecodedMessage
    {
        public string Result { get; private set; }  

        public QRDecodedMessage(string result)
        {
            Result = result;
        }        
    }
}
