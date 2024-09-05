namespace Nop.Web.Themes.SaljiDalje.Data;

public record Center(
    double x,
    double y
);

public record Crop(
    Center center,
    Flip flip,
    int rotation,
    int zoom,
    int aspectRatio
);

public record Flip(
    bool horizontal,
    bool vertical
);

public record Metadata(
    Crop crop,
    Resize resize,
    Output output
);

public record Output(
    object type,
    object quality,
    IReadOnlyList<string> client
);

public record Resize(
    string mode,
    bool upscale,
    Size size
);

public record FilePond(
    string id,
    string name,
    string type,
    int size,
    Metadata metadata,
    string data
);

public record Size(
    int width,
    int height
);