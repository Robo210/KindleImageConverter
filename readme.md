Manga Converter for Kindle
==========================

Converts manga, comics, and normal images into grayscale, Kindle compatible folders. It was inspired by [Mangle](http://foosoft.net/projects/mangle/), with the goal of being faster and producing better quality output.

Comparison to Mangle
--------------------

##### Pros
 - Faster. Convert images using all your CPU cores.
 - Better quality dithering. Color images are converted into grayscale using high-quality dithering algorithms that better preserve shading.
 - Zip/cbz support. Convert directly from zip files without having to extract them first.
 - Newer Kindle support. There are options to output for the Kindle Keyboard, Paperwhite, and Voyage.

##### Cons
 - Less options. There are currently no options to disable dithering or resizing, or to add borders around images.
 - Windows only.
 - The current UI is a bit silly.

In the comparison images below, notice how blotchy the face and clouds on the left are in the output from Mangle compared to the output from this software.

### Original
![Original](Resources\original.png)

### Manga Converter for Kindle
![Manga Converter for Kindle](Resources\kic.png)

### Mangle
![Mangle](Resources\mangle.png)

Compiling
---------

The first time you compile this software you will need to enable NuGet to download its dependencies.

Downloads
---------

There are currently no pre-compiled binaries available.
