using System;
using System.Data;
using Dapper;

namespace Kangaroo.UI.Services.Database;

/// <summary>
/// Facilitates the conversion from the GUID to a string and back. 
/// </summary>
public class GuidTypeHandler : SqlMapper.TypeHandler<Guid>
{
    public override void SetValue(IDbDataParameter parameter, Guid guid)
    {
        parameter.Value = guid;
    }

    public override Guid Parse(object value)
    {
        return new Guid((string)value);
    }
}