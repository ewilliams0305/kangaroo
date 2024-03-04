namespace Kangaroo.Queries;

internal interface IQueryFactory
{
    IQueryNetworkNode CreateQuerier(ScannerOptions? options = null);
}