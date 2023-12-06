using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tgs.BrandGameCredits.Handler.Core.ApplicationServices;

namespace Tgs.BrandGameCredits.Handler.Core;

public interface IHandlerDispatcher
{
    ValueTask PushCommandAsync(DispatcherCommand command, CancellationToken cancellationToken = default);
}
