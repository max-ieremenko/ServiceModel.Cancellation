namespace ServiceModel.Cancellation.Service
{
    internal interface IDispatchOperationManager
    {
        OperationInfo BeforeCall(string operationName, CancellationTokenProxy? token);

        void AfterCall(OperationInfo operation);
    }
}