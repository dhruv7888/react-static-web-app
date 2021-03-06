import React, { useState, useCallback, useEffect } from "react";
import ReactFlow, {
  addEdge,
  useNodesState,
  useEdgesState,
  ConnectionLineType,
} from "react-flow-renderer";
import {
  BrowserRouter as Router,Switch,Route,
  Redirect,Link,
} from "react-router-dom";
import "./Style.css";
import { nodes as initialNodes, edges as initialEdges } from "./elements";

function Identity() {
  let result           = "";
  let characters       = "ABCDEFGHIJKLMNOPQRSTUVWXYZ9";
  let charactersLength = characters.length;
  for ( let i = 0; i < 20; i++ ) {
    result += characters.charAt(Math.floor(Math.random() * 
charactersLength));
 }
 return result;
}

function ReactFlowRenderer(props) {
  console.log(props.name);
 //const elements=JSON.parse(props.name);
  console.log(typeof(props.name));
  
  const elements=props.name;
  const node=props.parnode;
  
  var locy =70;
  let locx=120;
  var cnt=1;
  
  const [nodes, setNodes, onNodesChange] = useNodesState([
  {
    id: "PARID",
    data: { label: <div><p>{node[0]}<br/>{node[1]}</p></div> },
    position: {
      x: (elements.length)*60+240,
      y: locy*cnt++,
    },
    style: {
      background: "#D6D5E6",
      color: "#333",
      border: "1px solid #222138",
      width: 200,
      height: 50,
    },    
  }]);
  const [edges, setEdges, onEdgesChange] = useEdgesState([]);
  

   const onConnect = useCallback(
    (params) =>
      setEdges((eds) =>
        addEdge(
          {
            ...params,
            type: ConnectionLineType.SmoothStep,
            animated: true,
            style: { stroke: "red" },
          },
          eds
        )
      ),
    [setEdges]
  );
 

  const addNode=(name)=>{
    setNodes((nds) => nds.concat({
      id: name[3],
      data: { label: <div><p>{name[0]}<br/>{name[1]}<br/>{name[2]}</p></div> },
      position: {
        x: locx*cnt,
        y: locy*cnt++,
      },
      style: {
        background: "#D6D5E6",
        color: "#333",
        border: "1px solid #222138",
        width: 200,
        height: 50,
      },    
    }));
  };
  
  const addEdges=(name)=>{
    setEdges((eds) => eds.concat({
      id: Identity(),
      source: name[0],
      target: name[1],
      type: ConnectionLineType.SmoothStep,
      animated: true,
      style: { stroke: "blue" },
    }));
  };

  const GetNodes=()=>{
    elements.map((item)=>
    {
      let id=Identity();
      let key=[item.ExternalAPIName,item.ExternalCallType,item.ExternalServiceName,id];
      addNode(key);
      let ed=["PARID",id];
      addEdges(ed);
    });
  }
  console.log(edges);

  useEffect(()=>{
      GetNodes();
  },[]);
  console.log(nodes);
  console.log(edges);
  function onInit() {
    console.log("Logged");
  }
  return (
    <>
    <div style={{ height: "100vh", margin: "0px" }}>
      <ReactFlow
        nodes={nodes}
        edges={edges}
        onNodesChange={onNodesChange}
        onEdgesChange={onEdgesChange}
        onConnect={onConnect}
        onInit={onInit}
        fitView
        attributionPosition="bottom-left"
        connectionLineType={ConnectionLineType.SmoothStep}
      />
    </div>
    </>
  );
}

export default ReactFlowRenderer;
