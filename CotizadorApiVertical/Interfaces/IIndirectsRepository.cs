using CotizadorApiVertical.Models;
using CotizadorApiVertical.Params;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CotizadorApiVertical.Interfaces
{
    public interface IIndirectsRepository
    {
        ResultOperationModel InsertIndirect(SqlConnection connection, SqlTransaction transaction, List<Viatico> viaticos, int cotizacionId);
        ResultOperationModel GetIndirect(int cotizacionId);
    }
}
