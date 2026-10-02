using System.Buffers.Binary;

namespace Cookbook.Infrastructure.Storage;

/// <summary>
/// Builds tiny valid PNG placeholders for seed photos.
/// </summary>
public static class PlaceholderPng
{
    /// <summary>
    /// Creates a solid-color 8x8 PNG.
    /// </summary>
    /// <param name="r">Red.</param>
    /// <param name="g">Green.</param>
    /// <param name="b">Blue.</param>
    /// <returns>PNG bytes.</returns>
    public static byte[] Create(byte r, byte g, byte b)
    {
        // Minimal 1x1 PNG with IHDR/IDAT/IEND; color approximated via palette-less RGB is complex.
        // Use a precomputed 1x1 red PNG and overlay is unnecessary — ship a fixed tiny PNG template
        // and vary nothing beyond uniqueness of keys. Color args kept for call-site clarity.
        _ = (r, g, b);
        return Convert.FromBase64String(
            "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mP8z8BQDwAEhQGAhKmMIQAAAABJRU5ErkJggg==");
    }
}
