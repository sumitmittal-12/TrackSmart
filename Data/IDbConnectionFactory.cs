using System.Data;

namespace TrackSmart.Data
{
    public interface IDbConnectionFactory
    {
        IDbConnection CreateConnection();
    }
}
