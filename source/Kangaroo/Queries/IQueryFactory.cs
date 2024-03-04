namespace Kangaroo.Queries;

internal interface IQueryFactory
{
    IQueryNetworkNode CreateQuerier();
}