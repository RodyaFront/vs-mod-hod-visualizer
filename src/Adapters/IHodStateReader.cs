using HodVisualizer.Contracts;
using Vintagestory.API.Client;

namespace HodVisualizer.Adapters;

internal interface IHodStateReader
{
    HodPlayerSnapshot Read(ICoreClientAPI capi);
}
