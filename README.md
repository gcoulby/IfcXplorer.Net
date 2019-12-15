# IfcXplorer
A text reader specifically designed for reading Industry Foundation Class (IFC) files.

I made this program when I was working as a research assistant, working predominantly with IFC files. 

Software such as Xbim and Solibri make it easier to view the Geometry, but there are times when you need to navigate the STEP21 files. To an extent Notepad++ makes this job easier. However, to read IFCZip files you have to first convert the file to .zip, then extract the IFC and open it in Notepad++. The primary purpose of this program is to make it easier to read IFCZips and it directly opens up the bytestream.

The second common action I found myself doing with STEP21 files was cross referencing IFC Entities. This project includes a project browser (experimental) powered by Xbim. It also converts all IFC Entity references in the file to clickable Hashtags. 
