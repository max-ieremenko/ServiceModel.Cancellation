namespace ServiceModel.Cancellation.Client
{
    internal interface IClientOperationManager
    {
        OperationInfo BeforeCall(string operationName, CancellationTokenProxy? token);

        void AfterCall(OperationInfo operation);
    }
}