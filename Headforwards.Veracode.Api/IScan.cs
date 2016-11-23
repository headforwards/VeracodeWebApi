using System;

namespace Headforwards.Veracode.Api
{
    public interface IScan
    {
        string Id { get; set; }
        string Name { get; set; }
        DateTime ScanDate { get; set; }
    }
}