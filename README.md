# sudhir-philips-assignment
Analyse text line by line and create an xml document based on the rules.

Presumptions taken:
- 	Only *.txt would be considered as a valid  input file
-	Only few validations has been applied for file. viz. file need to exists
-	Only "0 @I0001@ INDI" has been considered for an Id cadidate and is rendered as <indi id="@I0001@">. "2 INDI @I0001@" has been considered as a normal element and would be rendered as <indi>@I0001@</indi>

-	Application consists for following projects
	-	Philips.GDC.Console
		-	It works as starting point for the projects
		-	appSettings.json contains the following keys, both are optional and is having a default value of "MaxXmlProcessor" :1 and "RootNodeName":"gedcom" respectively
			{
			  "MaxXmlProcessor": 2,
			  "RootNodeName": "gedcom"
			}
		-	To run the application set this project as statup project.
		-	Program.cs act a starting point for the application. All services are configured here along with dependecny injection.
		
	-	Philips.GDC.Dto
		-	Contains Dto and Enums for the project
		
	-	Philips.GDC.Interface
		-	Contains contracts for the project
		
	-	Philips.GDC.Lexical
		-	Implements the various functionality of the project;
		
	-	Philips.GDC.Test
		-	Contains unit test cases for the application
		
	-	Philips.GDC.Utilities
		-	Contains common functionality such as configuration wrapper
		
-	Running application and expected output
	-	When you run the application, you would asked to provide filePath (To exist application simply enter Exit) as shown below:
			Enter input nodes in string format:
	-	Once after entering the filePath and pressing the enter would give you the following pattern of output
		-	Enter input nodes in string format:
		-	0 @I1@ INDI
		-	1 NAME Sudhir /Kumar/
		-	2 SURN Kumar
		-	2 GIVN Sudhir
		-	1 SEX M
		-	Ctrl + Z, Enter

		-	info: Philips.GDC.Lexical.LexicalController[0]
		-		  Processing file: D:\POC\Files\test1.txt
		-	info: Philips.GDC.Lexical.LexicalController[0]
		-		  0 @I1@ INDI
		-	info: Philips.GDC.Lexical.LexicalController[0]
		-	info: Philips.GDC.Lexical.LexicalController[0]
		-		  1 NAME Sudhir /Kumar/
		-	info: Philips.GDC.Lexical.LexicalController[0]
		-	info: Philips.GDC.Lexical.LexicalController[0]
		-	info: Philips.GDC.Lexical.LexicalController[0]
		-		  2 SURN Kumar
		-	info: Philips.GDC.Lexical.LexicalController[0]
		-		  2 GIVN Sudhir
		-	info: Philips.GDC.Lexical.LexicalController[0]
		-		  1 SEX M
		-	Enter file path (Exit to terminate the application):
		-	info: Philips.GDC.Lexical.TextFileProcessor[0]
		-		  Writing following content to xml file: 
		-		<gedcom>
		-			<indi id="@I1@">
		-			  <name value="Sudhir /Kumar/">
		-				<surn>Kumar</surn>
		-				<givn>Sudhir</givn>
		-			  </name>
		-			  <sex>M</sex>
		-			</indi>
		-		</gedcom>
	-	You can also run the application by downloading artifacts and running "Philips.GDC.Console.exe" 
		