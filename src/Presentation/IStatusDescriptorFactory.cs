using HodVisualizer.Domain;
using PlayerStatusStrip;
using Vintagestory.API.Client;

namespace HodVisualizer.Presentation;

internal interface IStatusDescriptorFactory
{
    StatusDescriptor Create(ICoreClientAPI capi, in StatusSignal signal);
}
