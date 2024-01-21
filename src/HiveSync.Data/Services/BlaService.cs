using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiveSync.Data.Services;

public interface IBlaService;
public class BlaService : IBlaService, ITransientService
{
}
