import './App.css';
import React, { Component } from 'react'
import Card from './Components/Card';
import { FcDocument, FcFile } from 'react-icons/fc'
import { BiError } from 'react-icons/bi'
import Tooltip from 'react-bootstrap/Tooltip'
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
      imgWidth: 0
    }
  };

  editorDidMount(editor, monaco) {
    editor.focus();
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
      reader.readAsDataURL(e.dataTransfer.files[0])
      reader.onload = () => {
        this.setState({ analyzing: true }
          ,() => axios.post("https://localhost:44343/api/document/analyze", { ImageAsBase64: reader.result } )
            .then(res => {
              result.push({ name, analyzedDocument: res.data.AnalyzedDocument, imageBase64: reader.result })
              this.setState({ result, selectedDocumentIndex: 0, analyzing: false })
            }))
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

          this.setState({ result, error: null })
        } 
        else if (res.data.Message) {
          this.setState({ error: res.data.Message })
        }
      })
  }

  onKeyDown = (newValue, e) => {
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
    if (img) {
      this.setState({
        heightCoefficient: (img.target.offsetHeight / img.target.naturalHeight),
        widthCoefficient: (img.target.offsetWidth / img.target.naturalWidth),
        imgHeight: img.target.offsetHeight,
        imgWidth: img.target.offsetWidth
      })
    }
  }

  fileOverlay = (result, selectedDocumentIndex, widthCoefficient, heightCoefficient) =>(
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
                          top: y[0] && y[0].BoundingBox.Y * heightCoefficient + 32 + 'px', 
                          left: y[0] && y[0].BoundingBox.X * widthCoefficient + 6 + 'px', 
                          fontSize: '15px', 
                          width: y[0] && y[0].BoundingBox.Width ? y[0].BoundingBox.Width * widthCoefficient + 5: 0, 
                          height: y[0] && y[0].BoundingBox.Height ? y[0].BoundingBox.Height * heightCoefficient + 5 : 0,
                          border: '1px solid rgba(0,0,0,0.05)'
                      }}/>
                  </OverlayTrigger>
                </div>
            )
          )
  )

  render() {
    const { analyzing, result, selectedDocumentIndex, script, error, dragOver, heightCoefficient, widthCoefficient } = this.state
    
    const codeContent = (
      <MonacoEditor 
        onChange={this.onKeyDown} 
        editorDidMount={this.editorDidMount}
        id="CodeArea"
        className="CodeArea"
        language="javascript"
        value={script}
      />
    )
    
    const overlay = this.fileOverlay(result, selectedDocumentIndex, widthCoefficient, heightCoefficient)

    const fileContent = (
      <div className="DropFileArea col-12" 
        onDragOver={this.dragOver}
        onDragEnter={this.dragEnter}
        onDragLeave={this.dragLeave}
        onDrop={this.fileDrop}
      >
          <Tabs
            id="controlled-tab-example"
            activeKey={selectedDocumentIndex}
            onSelect={this.onTabSelect}
          >
            {result.map((x, i) => x && <Tab className="FileTab" eventKey={i} title={x.name} />)}
            {analyzing ? <Tab className="FileTab" eventKey={selectedDocumentIndex} title='Analyzing ... ' /> : ''}
          </Tabs>
        {
          !result || 
          !(result[selectedDocumentIndex] 
            && result[selectedDocumentIndex].imageBase64) 
              ? <FcFile className={`DropFileImage ${analyzing ? 'rotate360' : (dragOver ? 'rotate5' : '')}`} /> 
              : <div><img onLoad={this.onLoad} className={`Fill`} src={result[selectedDocumentIndex].imageBase64} />{overlay && overlay.flat()}</div>
        }
      </div>
    )

    const resultContent = (
      <div>
        {result && result[selectedDocumentIndex] && result[selectedDocumentIndex].data && result[selectedDocumentIndex].data.split("\r\n").map(x => {
          const margin = `${x.indexOf('"')}`
          return <span className={`indent${margin == -1 ? 2 : margin == 4 ? 4 : 2}`}>{x}<br/></span>
        })}
      </div>
    )

    return (
      <div className="App container-fluid FullWidth">
        <div className="Top">
          <span><FcDocument /></span>
          <span>DocumentLab editor</span>
        </div>
        <div className="MainArea FullHeight">
          <div className="Row Flex FullHeight">
            <div className="col-7 FullWidth CodeArea">
              <Card 
                title="Code" 
                subtitle="Write your script here"
                warning={error && <OverlayTrigger
                  placement="right"
                  delay={{ show: 250, hide: 400 }}
                  overlay={(props) => (
                    <Tooltip id="ErrorTooltip" {...props}>
                      <div>
                        {error}
                      </div>
                    </Tooltip>
                  )}
                >
                  <BiError stlye={{marginLeft: 30, textSize: 30}}/>
                </OverlayTrigger>}
                content={codeContent} fullHeight 
              />
            </div>
            <div className="col-5 FullWidth">
              <Card title="File upload" subtitle="drag and drop your file here" content={fileContent} />
            </div>
          </div>
        </div>
        <div className="FullWidth ResultArea">
          <Card title="Result" subtitle="your interpretation results will show up here" content={resultContent} />
        </div>
      </div>
    );
  }
}

export default App;
