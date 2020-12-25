import './App.css';
import React, { Component } from 'react'
import { FcDocument, FcFile } from 'react-icons/fc'
import { BiError } from 'react-icons/bi'
import Tooltip from 'react-bootstrap/Tooltip'
import Table from 'react-bootstrap/Table'
import axios from 'axios';
import { OverlayTrigger, Tabs, Tab } from 'react-bootstrap';
import MonacoEditor from 'react-monaco-editor';

class App extends Component {

  constructor() {
    super()
    this.state = {
      analyzing: null,
      selectedDocumentIndex: 0,
      result: [],
      script: "",
      typingTimeout: 0,
      error: null,
      focusOffset: 0,
      dragOver: false,
      heightCoefficient: 0,
      widthCoefficient: 0,
      imgHeight: 0,
      imgWidth: 0,
      offsetLeft: 0,
      editorDecorations: []
    }
    this.imgRef = React.createRef();
    this.editorContainer = React.createRef();
    this.editor = React.createRef();
  };


  componentDidMount() {
    window.addEventListener('resize', this.updateDimensions);

    setTimeout(() => this.updateDimensions(), 100)
  }
  componentWillUnmount() {
    window.removeEventListener('resize', this.updateDimensions);
  }

  updateDimensions = () => {
    this.onLoad(this.imgRef.current)
    if (this.editor.current){
      const x = this.editorContainer.current.clientWidth
      const y = this.editorContainer.current.clientHeight - this.editorContainer.current.offsetTop - 10
      this.editor.current.editor.layout({width: x, height: y});
    }
  };

  editorWillMount(monaco) {
    axios.get("https://localhost:44343/api/document/texttypes").then(res => {
      monaco.languages.register({ id: 'documentlab' });
      monaco.languages.setMonarchTokensProvider('documentlab', {
        keywords: [ ...res.data.TextTypes, 'Right', 'Down', 'Left', 'Up', 'Any', 'RD', 'Table', 'Subset', 'Top', 'Bottom' ],
       
         operators: [
           '||'
         ],
       
         // we include these common regular expressions
         symbols:  /[=><!~?:&|+\-*\/\^%]+/,
       
         // C# style strings
         escapes: /\\(?:[abfnrtv\\"']|x[0-9A-Fa-f]{1,4}|u[0-9A-Fa-f]{4}|U[0-9A-Fa-f]{8})/,
       
         // The main tokenizer for our languages
         tokenizer: {
           root: [
             // identifiers and keywords
             [/[(a-z_$)|(A-Z_$)][\w\$]*\:/, 'type.identifier' ],  // to show class names nicely
             [/[(a-z_$)|(A-Z_$)][\w$]*/, { cases: { '@keywords': 'keyword', '@default': 'identifier' } }],
       
             // whitespace
             { include: '@whitespace' },
       
             // numbers
             [/\d*\.\d+([eE][\-+]?\d+)?/, 'number.float'],
             [/0[xX][0-9a-fA-F]+/, 'number.hex'],
             [/\d+/, 'number'],
       
             // delimiter: after number because of .\d floats
             [/[;,.]/, 'delimiter'],
       
             // strings
            [/'([^'\\]|\\.)*$/, 'string.invalid' ],  // non-teminated string
            [/'/,  { token: 'string.quote', bracket: '@open', next: '@string' } ],
           ],
       
           comment: [
             [/[^\/*]+/, 'comment' ],
             [/\/\*/,    'comment', '@push' ],    // nested comment
             ["\\*/",    'comment', '@pop'  ],
             [/[\/*]/,   'comment' ]
           ],
       
            bracketCounting: [
              [/[(a-z_$)|(A-Z_$)|(\s*)][\w\$]*\:/, 'delimiter.bracket', '@push'],
              [/[(a-z_$)|(A-Z_$)|(\s*)|\]][\w\$]*\;/, 'delimiter.bracket', '@pop'],
              { include: 'root' }
            ],
  
            string: [
              [/[^\\']+/,  'string'],
              [/@escapes/, 'string.escape'],
              [/\\./,      'string.escape.invalid'],
              [/'/,        { token: 'string.quote', bracket: '@close', next: '@pop' } ]
            ],
  
           whitespace: [
             [/[ \t\r\n]+/, 'white'],
             [/\/\*/,       'comment', '@comment' ],
             [/\/\/.*$/,    'comment'],
           ]
         }
      });
    })
  }

  editorDidMount(editor, monaco) {
    editor.focus();
        
        editor.trigger(3, null, { })
        editor.trigger(2, null, { })
  }

  dragOver = (e) => {
    e.preventDefault();
    this.setState({ dragOver: true })
  }

  dragEnter = (e) => {
      e.preventDefault();
  }

  dragLeave = (e) => {
      e.preventDefault();
      this.setState({ dragOver: false })
    }

  fileDrop = (e) => {
      e.preventDefault();
      const { result } = this.state
      const { name } = e.dataTransfer.files[0]
      const reader = new FileReader()

      this.setState({ analyzing: true }, () => {
        reader.readAsDataURL(e.dataTransfer.files[0])
  
        reader.onload = () => {
          axios.post("https://localhost:44343/api/document/analyze", { ImageAsBase64: reader.result } )
          .then(res => {
            result.push({ name, analyzedDocument: res.data.AnalyzedDocument, imageBase64: reader.result })
            this.setState({ result, selectedDocumentIndex: result.length - 1, analyzing: false })
          })
        }
      })
  }

  clearDecorations = (onlyErrors = false) => {
    var elems = document.querySelectorAll(".errorCodeLine");
    var elems2 = document.querySelectorAll(".errorGlyphMargin");
    var elems4 = document.querySelectorAll(".error");
    
    if (!onlyErrors) {
      var elems3 = document.querySelectorAll(".validResultGlyphMargin");
      var elems5 = document.querySelectorAll(".result");

      if (elems3 && elems3.length > 0) {
        [].forEach.call(elems3, function(el) {
          el.remove();
        });
      }

      if (elems5 && elems5.length > 0) {
        [].forEach.call(elems5, function(el) {
          el.remove();
        });
      }
    }

    if (elems && elems.length > 0) {
      [].forEach.call(elems, function(el) {
        el.classList.remove("errorCodeLine");
      });
    }

    if (elems2 && elems2.length > 0) {
      [].forEach.call(elems2, function(el) {
        el.remove();
      });
    }

    if (elems4 && elems4.length > 0) {
      [].forEach.call(elems4, function(el) {
        el.remove();
      });
    }
  }

  addGlyph = (lineNumber, character, className) => {
    var div = document.createElement('div', )
    div.innerHTML = character;
    div.className = `cgmr codicon ${className}`;

    var lineNumbers = document.getElementsByClassName('margin-view-overlays')[0]

    if (lineNumbers) {
      for (var i = 0; i < lineNumbers.children.length; i++) {
        if (lineNumbers.children[i].getElementsByTagName('div')[0].innerHTML == lineNumber)
        {
          lineNumbers.children[i].appendChild(div)
            break;
        }
      }
    }
  }

  addResults = (results) => {
    if(!results) {
      return; 
    }

    var lines = document.getElementsByClassName('view-lines')[0]
    var children = lines.children

    for (var i = 0; i < children.length; i++) {
      var spans = children[i].getElementsByTagName('span');

      if (spans && spans.length > 1) {
        var lineSpan = spans[1]
        if (lineSpan.innerHTML)
        {
          var line = lineSpan.innerHTML.substring(0, lineSpan.innerHTML.indexOf(":"))
    
          if (results[line])
          {
            var span = document.createElement('span', )
            span.innerHTML = JSON.stringify(results[line])
            span.className = 'result'
            spans[0].appendChild(span)
          }
        }
      }
    }         
  }

  addErrors = (lineNumber, error) => {
    var lines = document.getElementsByClassName('view-lines')[0]
    var children = lines.children
    const sorted = [...children].sort((a, b) => a.style.top - b.style.top)
    var spans = sorted[lineNumber - 1].getElementsByTagName('span');

    if (spans && spans.length > 1) {
      var lineSpan = spans[1]
      if (lineSpan.innerHTML)
      {
        var line = lineSpan.innerHTML.substring(0, lineSpan.innerHTML.indexOf(":"))
  
        var span = document.createElement('span', )
        span.innerHTML = JSON.stringify(error.substring(error.indexOf("-")))
        span.className = 'error'
        spans[0].appendChild(span)

      }
    }
  }

  interpretScript = () => {
    const { script, result, selectedDocumentIndex } = this.state

    if (!result || ! result[selectedDocumentIndex] || !result[selectedDocumentIndex].analyzedDocument)
      return;

    axios.post("https://localhost:44343/api/document/interpret", { AnalyzedDocument: result[selectedDocumentIndex].analyzedDocument, Script: script })
      .then(res => {
        if (res.data.InterpretedDocument) {
          result[selectedDocumentIndex] = { ...result[selectedDocumentIndex], data: res.data.InterpretedDocument }
          this.clearDecorations()
          
          this.addResults(JSON.parse(result[selectedDocumentIndex].data))
          
          const lines = this.state.script.split('\n');
          const mapped = lines.map((x, i) => ({ word: x.substring(0, x.indexOf(':')), line: i } ))
          const lineNumbersDict = Object.assign({}, ...mapped.map((x) => ({[x.word]: x.line})));
          const keys = Object.keys(JSON.parse(result[selectedDocumentIndex].data));
          Object.keys(JSON.parse(result[selectedDocumentIndex].data)).forEach(x => {
            this.addGlyph(lineNumbersDict[x] + 1, '✓', 'validResultGlyphMargin')
          })

          this.setState({ result, error: null })
        } 
        else if (res.data.Message) {
          this.setState({ error: res.data.Message },
            () => {

              const lineMatches = this.state.error.match(/Line\:\s*(\d*)/)
              const characterMatches = this.state.error.match(/Char\:\s*(\d*)/)
              if (lineMatches && lineMatches.length > 1 && characterMatches && characterMatches.length > 1)
              {
                const lineNumber = lineMatches[1]
                const lineStart = this.state.script.split('\n')[lineNumber - 1].lastIndexOf(' ') + 1
                const lineEnd = characterMatches[1] + 1
                
                if (this.editor && this.editor.current.editor) {
                  this.addErrors(lineNumber, res.data.Message);
                  this.addGlyph(lineNumber, 'X', 'errorGlyphMargin');
                }
              }
            })
        }
      })
  }

  onKeyDown = (newValue, e) => {
    this.clearDecorations(true);
    const currentScript = newValue

    this.setState({script: currentScript},
      () => {
        if (this.state.typingTimeout) {
          clearTimeout(this.state.typingTimeout);
        }
    
      this.setState({
        typingTimeout: setTimeout(() => this.interpretScript(), 500)
      })
    })
  }

  onTabSelect = (key) => 
  {
    this.setState({ selectedDocumentIndex: key })
  }
  
  onLoad = (img) => {
    if (!img) 
      return

    if (img.target) 
      img = img.target

    if (img) {
      this.setState({
        offsetLeft: img.offsetLeft,
        heightCoefficient: (img.offsetHeight / img.naturalHeight),
        widthCoefficient: (img.offsetWidth / img.naturalWidth),
        imgHeight: img.offsetHeight,
        imgWidth: img.offsetWidth
      })
    }
  }

  fileOverlay = (result, selectedDocumentIndex, widthCoefficient, heightCoefficient, offsetLeft) =>(
    !result || !(result[selectedDocumentIndex] && result[selectedDocumentIndex].analyzedDocument) ? '' :
      result[selectedDocumentIndex]
        .analyzedDocument
        .Contents
        .map(x => x 
          && x.map(y => y 
              && <div>
                  <OverlayTrigger 
                    placement="left"
                    delay={{ show: 250, hide: 300 }}
                    overlay={(props) => (
                      <Tooltip
                        className="ResultToolTip"
                        id="InfoTooltip" {...props}
                      >
                        {y.map(z => <div>{z.TextType} - {z.Value}</div>)}
                      </Tooltip>
                      )}
                  >
                      <div style={
                        { 
                          position: 'absolute', 
                          top: y[0] && y[0].BoundingBox.Y * heightCoefficient - 3 + 'px', 
                          left: y[0] && y[0].BoundingBox.X * widthCoefficient - 10 + offsetLeft + 'px', 
                          fontSize: '15px', 
                          width: y[0] && y[0].BoundingBox.Width ? y[0].BoundingBox.Width * widthCoefficient + 5: 0, 
                          height: y[0] && y[0].BoundingBox.Height ? y[0].BoundingBox.Height * heightCoefficient + 5 : 0,
                          border: '1px solid rgba(0,0,0,0.2)'
                      }}/>
                  </OverlayTrigger>
                </div>
            )
          )
  )

  render() {
    const { analyzing, result, selectedDocumentIndex, script, error, dragOver, heightCoefficient, widthCoefficient, offsetLeft } = this.state
    
    const requireConfig = {
      url: 'node_modules/monaco-editor/min/vs/loader.js',
      paths: {
        vs: 'node_modules/monaco-editor/min/vs'
      }
    };
    
    const codeContent = (
        <div className="FullHeight CodeCard" ref={this.editorContainer}>
        <MonacoEditor 
          onChange={this.onKeyDown} 
          editorDidMount={this.editorDidMount}
          editorWillMount={this.editorWillMount}
          ref={this.editor}
          language="documentlab"
          value={script}
          glyphMargin={true}
          theme="vs-dark"
          requireConfig={requireConfig}
        />
        </div>
    )
    
    const overlay = this.fileOverlay(result, selectedDocumentIndex, widthCoefficient, heightCoefficient, offsetLeft)

    const fileContent = (
      <div className="Card DropFileArea" 
        onDragOver={this.dragOver}
        onDragEnter={this.dragEnter}
        onDragLeave={this.dragLeave}
        onDrop={this.fileDrop}
      >
        {
          !result || 
          !(result[selectedDocumentIndex] 
            && result[selectedDocumentIndex].imageBase64
            && !analyzing) 
              ? <FcFile className={`DropFileImage ${analyzing ? 'rotate360' : (dragOver ? 'rotate5' : '')}`} /> 
              : <div style={{ display: "Flex", justifyContent: "Center" }}>
                  <img 
                    onLoad={this.onLoad} 
                    className={`Fill`} 
                    src={result[selectedDocumentIndex].imageBase64} 
                    ref={this.imgRef} 
                    style={{maxWidth: "100%", maxHeight: "1000px"}} 
                  />
                    {overlay && overlay.flat()}
                </div>
        }
      </div>
    )

    if (result && result[selectedDocumentIndex] && result[selectedDocumentIndex].data) {
      console.log(JSON.parse(result[selectedDocumentIndex].data))
    }

    const resultContent = (
      <div>
        {result && result[selectedDocumentIndex] && result[selectedDocumentIndex].data 
          && <Table>
              <thead>
                <tr>
                  {Object.keys(JSON.parse(result[selectedDocumentIndex].data)).map(x => <th>{x}</th>)}
                </tr>
              </thead>
              <tbody>
                {Object.values(JSON.parse(result[selectedDocumentIndex].data)).map(x => <tr><td>{x}</td></tr>)}
              </tbody>
            </Table>
        }
      </div>
    )

    return (
      <div className="App">
        {codeContent}
        {fileContent} 
      </div>
    );
  }
}

export default App;
