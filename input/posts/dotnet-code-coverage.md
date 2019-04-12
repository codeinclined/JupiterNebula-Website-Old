Title: TabPanel Test
Published: 4/2/2019
Tags:
  - unit testing
  - .NET Core
  - Azure DevOps
---

## Code in a TabPanel

<?# TabBlock ?>
- C#
  ```csharp
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Xml;
  using System.Xml.Linq;
  using shortid;
  using Wyam.Common.Documents;
  using Wyam.Common.Execution;
  using Wyam.Common.Shortcodes;

  namespace JupiterNebula.Wyam.Shortcodes.TabBlock
  {
      public class TabBlock : IShortcode
      {
          private static string CreateBaseId() => $"{nameof(TabBlock)}__{ShortId.Generate(true, false)}";
          private static string CreateTabId(string baseId, int tabNumber) => $"{baseId}-{tabNumber}";
          private static string CreateTabLinkId(string tabId) => tabId + "-link";
          private static string CreateTabPaneId(string tabId) => tabId + "-pane";

          #region Tab List
          private static XElement CreateTabLink(XNode labelNode, bool active, string tabId)
          {
              var linkElement = new XElement("a", labelNode);
              var tabPaneId = CreateTabPaneId(tabId);

              linkElement.SetAttributeValue("class", $"nav-link{(active ? " active" : string.Empty)}");
              linkElement.SetAttributeValue("data-toggle", "tab");
              linkElement.SetAttributeValue("aria-selected", XmlConvert.ToString(active));
              linkElement.SetAttributeValue("aria-controls", tabPaneId);
              linkElement.SetAttributeValue("href", '#' + tabPaneId);
              linkElement.SetAttributeValue("id", CreateTabLinkId(tabId));

              return linkElement;
          }

          private static XElement CreateTab(XNode labelNode, bool active, string tabId)
          {
              var tabElement = new XElement("li", CreateTabLink(labelNode, active, tabId));
              tabElement.SetAttributeValue("class", "nav-item");

              return tabElement;
          }

          private static XElement CreateTabList(IEnumerable<XElement> shortcodeTabs, 
              Func<XElement, XNode> labelSelector, string baseId)
          {
              int tabCount = 0;
              var tabList =  new XElement("ul", 
                  shortcodeTabs.Select(n => labelSelector(n))
                               .Select(n => CreateTab(n, tabCount == 0, CreateTabId(baseId, tabCount++))));
              
              tabList.SetAttributeValue("class", "nav nav-tabs");
              tabList.SetAttributeValue("role", "tablist");

              return tabList;
          }
          #endregion

          #region Tab Content
          private static XElement CreateTabPane(IEnumerable<XNode> contentNodes, bool active, string tabId)
          {
              var tabPane = new XElement("div", contentNodes);

              tabPane.SetAttributeValue("class", $"tab-pane{(active ? " show active" : string.Empty)}");
              tabPane.SetAttributeValue("role", "tabpanel");
              tabPane.SetAttributeValue("aria-labelledby", CreateTabLinkId(tabId));
              tabPane.SetAttributeValue("id", CreateTabPaneId(tabId));

              return tabPane;
          }

          private static XElement CreateTabPaneContainer(IEnumerable<XElement> shortcodeTabs, 
              Func<XElement, IEnumerable<XNode>> contentSelector, string baseId)
          {
              int tabCount = 0;

              var tabContent = new XElement("div", 
                  shortcodeTabs.Select(contentSelector)
                               .Select(n => CreateTabPane(n, tabCount == 0, CreateTabId(baseId, tabCount++))));

              tabContent.SetAttributeValue("class", "tab-content");

              return tabContent;
          }
          #endregion

          private static XElement CreateTabBlock(IEnumerable<XElement> shortcodeTabs, 
              Func<XElement, XNode> tabLabelSelector, Func<XElement, IEnumerable<XNode>> tabContentSelector)
          {
              var baseId = CreateBaseId();

              var tabBlock = new XElement("div", 
                  CreateTabList(shortcodeTabs, tabLabelSelector, baseId),
                  CreateTabPaneContainer(shortcodeTabs, tabContentSelector, baseId));

              tabBlock.SetAttributeValue("class", "tab-block");
              tabBlock.SetAttributeValue("id", baseId);

              return tabBlock;
          }

          public IShortcodeResult Execute(KeyValuePair<string, string>[] args, string content,
              IDocument document, IExecutionContext context)
          {
              var contentElement = XElement.Parse(content);
              var tabBlock = CreateTabBlock(contentElement.Elements("li"),
                  nameof => nameof.FirstNode, n => n.Nodes().Skip(1));
              
              return context.GetShortcodeResult(tabBlock.ToString(SaveOptions.DisableFormatting));
          }
      }
  }
  ```
- F#
  ```fsharp
  // Taken from: https://fsharpforfunandprofit.com/posts/fsharp-in-60-seconds/
  // single line comments use a double slash
  (* multi line comments use (* . . . *) pair -end of multi line comment- *)

  // ======== "Variables" (but not really) ==========
  // The "let" keyword defines an (immutable) value
  let myInt = 5
  let myFloat = 3.14
  let myString = "hello"	//note that no types needed

  // ======== Lists ============
  let twoToFive = [2;3;4;5]        // Square brackets create a list with
                                  // semicolon delimiters.
  let oneToFive = 1 :: twoToFive   // :: creates list with new 1st element
  // The result is [1;2;3;4;5]
  let zeroToFive = [0;1] @ twoToFive   // @ concats two lists

  // IMPORTANT: commas are never used as delimiters, only semicolons!

  // ======== Functions ========
  // The "let" keyword also defines a named function.
  let square x = x * x          // Note that no parens are used.
  square 3                      // Now run the function. Again, no parens.

  let add x y = x + y           // don't use add (x,y)! It means something
                                // completely different.
  add 2 3                       // Now run the function.

  // to define a multiline function, just use indents. No semicolons needed.
  let evens list =
    let isEven x = x%2 = 0     // Define "isEven" as an inner ("nested") function
    List.filter isEven list    // List.filter is a library function
                                // with two parameters: a boolean function
                                // and a list to work on

  evens oneToFive               // Now run the function

  // You can use parens to clarify precedence. In this example,
  // do "map" first, with two args, then do "sum" on the result.
  // Without the parens, "List.map" would be passed as an arg to List.sum
  let sumOfSquaresTo100 =
    List.sum ( List.map square [1..100] )

  // You can pipe the output of one operation to the next using "|>"
  // Here is the same sumOfSquares function written using pipes
  let sumOfSquaresTo100piped =
    [1..100] |> List.map square |> List.sum  // "square" was defined earlier

  // you can define lambdas (anonymous functions) using the "fun" keyword
  let sumOfSquaresTo100withFun =
    [1..100] |> List.map (fun x->x*x) |> List.sum

  // In F# returns are implicit -- no "return" needed. A function always
  // returns the value of the last expression used.

  // ======== Pattern Matching ========
  // Match..with.. is a supercharged case/switch statement.
  let simplePatternMatch =
    let x = "a"
    match x with
      | "a" -> printfn "x is a"
      | "b" -> printfn "x is b"
      | _ -> printfn "x is something else"   // underscore matches anything

  // Some(..) and None are roughly analogous to Nullable wrappers
  let validValue = Some(99)
  let invalidValue = None

  // In this example, match..with matches the "Some" and the "None",
  // and also unpacks the value in the "Some" at the same time.
  let optionPatternMatch input =
    match input with
      | Some i -> printfn "input is an int=%d" i
      | None -> printfn "input is missing"

  optionPatternMatch validValue
  optionPatternMatch invalidValue

  // ========= Complex Data Types =========

  // Tuple types are pairs, triples, etc. Tuples use commas.
  let twoTuple = 1,2
  let threeTuple = "a",2,true

  // Record types have named fields. Semicolons are separators.
  type Person = {First:string; Last:string}
  let person1 = {First="john"; Last="Doe"}

  // Union types have choices. Vertical bars are separators.
  type Temp = 
    | DegreesC of float
    | DegreesF of float
  let temp = DegreesF 98.6

  // Types can be combined recursively in complex ways.
  // E.g. here is a union type that contains a list of the same type:
  type Employee = 
    | Worker of Person
    | Manager of Employee list
  let jdoe = {First="John";Last="Doe"}
  let worker = Worker jdoe

  // ========= Printing =========
  // The printf/printfn functions are similar to the
  // Console.Write/WriteLine functions in C#.
  printfn "Printing an int %i, a float %f, a bool %b" 1 2.0 true
  printfn "A string %s, and something generic %A" "hello" [1;2;3;4]

  // all complex types have pretty printing built in
  printfn "twoTuple=%A,\nPerson=%A,\nTemp=%A,\nEmployee=%A" 
          twoTuple person1 temp worker

  // There are also sprintf/sprintfn functions for formatting data
  // into a string, similar to String.Format.
  ```
<?#/ TabBlock ?>

asdf