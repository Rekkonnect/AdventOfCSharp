namespace AdventOfCSharp;

/// <summary>
/// Provides the available options to handling overflowing the
/// console's buffer.
/// </summary>
public enum BufferHeightHandlingMode
{
    /// <summary>
    /// Do nothing to handle the overflow. This means that there
    /// may be an exception thrown, or nothing being printed to
    /// the console, depending on the operating system and the
    /// terminal that this program runs on.
    /// </summary>
    /// <remarks>
    /// This is not recommended for usage on standard platforms
    /// and terminals, including Windows, PowerShell, bash, etc.
    /// </remarks>
    None,
    /// <summary>
    /// Increase the buffer height sufficiently so that the new
    /// content can be printed. Not recommended, as successive
    /// invocations may slow performance down.
    /// </summary>
    /// <remarks>This is not yet implemented.</remarks>
    [Obsolete("This is not implemented; read the documentation for more info.", true)]
    IncreaseBufferHeight,
    /// <summary>
    /// Increase the buffer height to the maximum value. This
    /// still does not guarantee going past the maximum value.
    /// This approach should suffice for most cases of normal
    /// usage of the program.
    /// </summary>
    /// <remarks>
    /// This is only available on Windows. Other platforms do
    /// not support adjusting the buffer height.
    /// </remarks>
    MaximizeBufferHeight,
    /// <summary>
    /// Clear the entire buffer and write the new content.
    /// This option will purposefully underestimate the remaining
    /// space to make room for the content that is intended to be
    /// printed. Previously important content may be lost and not
    /// reprinted on the buffer.
    /// </summary>
    ClearBuffer,
    /// <summary>
    /// Print newlines using <seealso cref="Console.WriteLine"/>
    /// until there is enough height to print the new content, or
    /// navigate the cursor around. Restrictingly low buffer
    /// heights may cause issues.
    /// </summary>
    /// <remarks>
    /// This is the default option for most platforms and
    /// terminals.
    /// </remarks>
    PrintNewlines,
}
