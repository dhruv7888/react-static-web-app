import React from "react"; 
export const nodes = [
    {
      id: "1",
      type: "input",
      data: {      
        label: <div>Text1 <br/>Text2</div>,
      },
      position: { x: 250, y: 0 },
    },
    {
      id: "2",
      data: {
        title: "some_source_data",
    content: "Data"
      },
      position: { x: 100, y: 100 },
    },
    {
      id: "3",
      data: {
        label: "Node 3\n s+nice ",
      },
      position: { x: 400, y: 100 },
      style: {
        background: "#D6D5E6",
        color: "#333",
        border: "1px solid #222138",
        width: 180,
        height: 200,
      },
    },
  ];
  
  export const edges = [
    { id: "e1-2", source: "1", target: "2", type: "smoothstep", animated: true },
    { id: "e1-3", source: "1", target: "3", type: "smoothstep", animated: true,markerStart:"1",markerEnd:"3" },
  ];
  