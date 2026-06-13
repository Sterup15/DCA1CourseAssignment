namespace VEA.Core.QueryContracts.Exceptions;

public class QueryHandlerNotFoundException(string queryType, string answerType)
    : Exception($"No query handler registered for query '{queryType}' with answer '{answerType}'.");