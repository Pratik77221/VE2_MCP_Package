namespace Imperial.VE2.MCP.Editor
{
    internal static class Response
    {
        public static object Success(string message, object data = null)
        {
            if (data == null)
            {
                return new
                {
                    success = true,
                    message
                };
            }

            return new
            {
                success = true,
                message,
                data
            };
        }

        public static object Error(string message)
        {
            return new
            {
                success = false,
                code = message,
                error = message
            };
        }
    }
}
