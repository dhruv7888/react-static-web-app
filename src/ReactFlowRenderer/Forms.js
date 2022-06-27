import React, {useState} from 'react';
import './Style.css';
import {
    BrowserRouter as Router,Link,
} from "react-router-dom";
import Header from './Header.js';
import ReactFlowRenderer from './ReactFlowRenderer';

function Forms() {
    const [DGrepEndpoint, setDGrepEndpoint] = useState("PublicCloud");
    const [MdsEndpoint, setMdsEndpoint] = useState("BillingPROD");
    const [Namespace, setNamespace] = useState(null);
    const [EventName,setEventName] = useState(null);
    const [ServiceName,setServiceName] = useState(null);
    const [APIName, setAPIName] = useState(null);
    const [View,setView] = useState("EnterDetails");    
    const [Data,setData] = useState(null);
    const [Node,setNode] = useState(null);
    
    const handleInputChange = (e) => {
        const {id , value} = e.target;
        if(id === "DGrepEndpoint"){
            setDGrepEndpoint(value);
        }
        else if(id === "MdsEndpoint"){
            setMdsEndpoint(value);
        }
        if(id === "Namespace"){
            setNamespace(value);
        }
        else if(id === "EventName"){
            setEventName(value);
        }
        else if(id === "ServiceName"){
            setServiceName(value);
        }
        else if(id === "APIName"){
            setAPIName(value);
        }      
    }
    const handleSubmit  = async() => {
        var data={};
        data["DGrepEndpoint"]=DGrepEndpoint;
        data["MdsEndpoint"]=MdsEndpoint;
        data["Namespace"]=Namespace;
        data["EventName"]=EventName;
        data["ServiceName"]=ServiceName;
        data["APIName"]=APIName;
        var json=JSON.stringify(data);
        await fetch(`/api/CodeFlow?name=`+json).then((response)=>{return response.json();}).then( data => setData(data)).catch(error=>console.log(error));    
        setNode([ServiceName,APIName]);
        setView("CodeFlow");
    }
    if(View==="EnterDetails")
    {
        return(
        <>
            <Header/>
            <div className="form">
                <div className="form-body">
                    <div className="DGrepEndpoint">
                        <label className="form__label" for="DGrepEndpoint">DGrepEndpoint : </label>
                        <select name="DGrepEndpoint" className="form__input" value={DGrepEndpoint} onChange = {(e) => handleInputChange(e)} id="DGrepEndpoint">
                            <option value="PublicCloud">Public cloud</option>
                            <option value="Mooncake">Mooncake</option>
                            <option value="Blackforest">Blackforest</option>
                            <option value="Fairfax">Fairfax</option>
                            <option value="USSec">USSec</option>
                            <option value="USNat">USNat</option>
                        </select>
                    </div>
                    <div className="MdsEndpoint">
                        <label className="form__label" for="MdsEndpoint">MdsEndpoint : </label>
                        <select name="MdsEndpoint" id="MdsEndpoint" className="form__input" value={MdsEndpoint} onChange = {(e) => handleInputChange(e)} >
                            <option value="BlackForest">Black Forest</option>
                            <option value="CAFairfax">CA Fairfax</option>
                            <option value="CAMooncake">CA Mooncake</option>
                            <option value="DiagnosticsPROD">Diagnostics PROD</option>
                            <option value="ExternalPROD">External PROD</option>
                            <option value="FirstPartyPROD">FirstParty PROD</option>
                            <option value="Stage">Stage</option>
                            <option value="Test">Test</option>
                        </select>
                    </div>
                    <div className="Namespace">
                        <label className="form__label" for="Namespace">Namespace : </label>
                        <input  type="text" id="Namespace" className="form__input" value={Namespace} onChange = {(e) => handleInputChange(e)} placeholder="Namespace"/>
                    </div>
                    <div className="EventName">
                        <label className="form__label" for="EventName">Event Name : </label>
                        <input className="form__input" type="text"  id="EventName" value={EventName} onChange = {(e) => handleInputChange(e)} placeholder="Event Name"/>
                    </div>
                    <div className="ServiceName">
                        <label className="form__label" for="ServiceName">Service Name : </label>
                        <input className="form__input" type="text" id="ServiceName" value={ServiceName} onChange = {(e) => handleInputChange(e)} placeholder="Service Name"/>
                    </div>
                    <div className="APIName">
                        <label className="form__label" for="APIName">API Name : </label>
                        <input className="form__input" type="text" id="APIName" value={APIName} onChange = {(e) => handleInputChange(e)} placeholder="API Name"/>
                    </div>
                    <div class="footer">
                        <button type="submit" class="btn btn2" onClick={()=>handleSubmit()} >Register</button>
                    </div>
                </div>
            </div>  
        </>
        );
    }
    else if(View==="CodeFlow")
    {
        return (<>
        <Header />
        <ReactFlowRenderer name={Data} parnode={Node}/>
        </>);
    }
}
export default Forms;
