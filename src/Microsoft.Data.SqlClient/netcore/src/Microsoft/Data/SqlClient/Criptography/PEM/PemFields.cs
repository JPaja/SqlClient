namespace Microsoft.Data.SqlClient.Criptography.PEM
{
    internal readonly struct PemFields
    {
        internal PemFields(int labelStart, int labelEnd, int base64dataStart, int base64dataEnd, int locationStart, int locationEnd, int decodedDataLength)
        {
            DecodedDataLength = decodedDataLength;

            LocationStart = locationStart;
            LocationLenght = locationEnd - locationStart;
            LocationEnd = locationEnd;

            LabelStart = labelStart;
            LabelLenght = labelEnd - labelStart;
            LabelEnd = labelEnd;

            Base64DataStart = base64dataStart;
            Base64DataLenght = base64dataEnd - base64dataStart;
            Base64DataEnd = base64dataEnd;
        }


        /// <summary>
        /// Gets the location start of the PEM-encoded text, including the surrounding encapsulation boundaries.
        /// </summary>
        public int LocationStart { get; }

        /// <summary>
        /// Gets the location lenght of the PEM-encoded text, including the surrounding encapsulation boundaries.
        /// </summary>
        public int LocationLenght { get; }

        /// <summary>
        /// Gets the location end of the PEM-encoded text, including the surrounding encapsulation boundaries.
        /// </summary>
        public int LocationEnd { get; }

        /// <summary>
        /// Gets the location start of the label.
        /// </summary>
        public int LabelStart { get; }

        /// <summary>
        /// Gets the location lenght of the label.
        /// </summary>
        public int LabelLenght { get; }

        /// <summary>
        /// Gets the location end of the label.
        /// </summary>
        public int LabelEnd { get; }

        /// <summary>
        /// Gets the location start of the base-64 data inside of the PEM.
        /// </summary>
        public int Base64DataStart { get; }

        /// <summary>
        /// Gets the location lenght of the base-64 data inside of the PEM.
        /// </summary>
        public int Base64DataLenght { get; }

        /// <summary>
        /// Gets the location end of the base-64 data inside of the PEM.
        /// </summary>
        public int Base64DataEnd { get; }

        /// <summary>
        /// Gets the size of the decoded base-64 data, in bytes.
        /// </summary>
        public int DecodedDataLength { get; }
    }
}
