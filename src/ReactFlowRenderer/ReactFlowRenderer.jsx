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
  
  const [nodes, setNodes, onNodesChange] = useNodesState([]);
  const [edges, setEdges, onEdgesChange] = useEdgesState([]);
  var loc =80;
  var cnt=1;

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
      id: name[2],
      data: { label: <div><p>{name[0]}<br/>{name[1]}</p></div> },
      position: {
        x: loc*cnt,
        y: loc*cnt++,
      },
      style: {
        background: "#D6D5E6",
        color: "#333",
        border: "1px solid #222138",
        width: 350,
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

  const elements=props.name;
  
  const GetNodes=()=>{
    var hashmap = new Map();
    var prev="";
    var cnt=0;
    elements.map((item)=>
      {
        let key1=item.Event_name+"_"+item.state;
        if (!(hashmap.has(key1)))
        {
          hashmap.set(key1,Identity());
          let key=[item.Event_name,item.state,hashmap.get(key1)];
          addNode(key);
        }
        if(cnt!==0)
        {
          let ed=[hashmap.get(prev),hashmap.get(key1)];
          addEdges(ed);
        }
        cnt++;
        prev=key1;
      });
    console.log(hashmap);
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
