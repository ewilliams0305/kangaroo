using System;
using System.Data;
using Dapper;

namespace Kangaroo.UI.Services.Database;

public class DateTimeTypeHandler : SqlMapper.TypeHandler<DateTime>
{
    public override void SetValue(IDbDataParameter parameter, DateTime dateTime)
    {
        parameter.Value = dateTime;
    }

    public override DateTime Parse(object value)
    {
        return DateTime.Parse(value.ToString() ?? string.Empty);
    }
}