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
    const [View,setView] = useState("EnterDetails");    
    const [Data,setData] = useState(null);
    const [Json,setJson] = useState(null);
    
    const handleInputChange = (e) => {
        const {id , value} = e.target;
        if(id === "Namespace"){
            setNamespace(value);
        }
        else if(id === "EventName"){
            setEventName(value);
        }
        else if(id === "DGrepEndpoint"){
            setDGrepEndpoint(value);
        }
        else if(id === "MdsEndpoint"){
            setMdsEndpoint(value);
        }
    }
    const handleSubmit  = () => {
        var data={};
        data["DGrepEndpoint"]=DGrepEndpoint;
        data["Namespace"]=Namespace;
        data["MdsEndpoint"]=MdsEndpoint;
        data["EventName"]=EventName;
        setData([{"Time": "2022-05-31T07:50:04.179Z","Event_name": "Start_EnterpriseReporting.Processor_ProcessCheckPoint","state": ""},{"Time": "2022-05-31T07:50:05.241Z","Event_name": "UpdateCheckpoint_EnterpriseReporting_ImportEnrollmentOperation","state": "Created"},{"Time": "2022-05-31T07:50:05.304Z","Event_name": "UpdateCheckpoint_EnterpriseReporting_ImportEnrollmentOperation","state": "PopulateBillingGroupOwner"},{"Time": "2022-05-31T07:50:04.179Z","Event_name": "Start_EnterpriseReporting.Processor_ProcessCheckPoint","state": ""}]);
        //setJson(JSON.stringify(Data));
        /*    fetch(`/api/hello?name=`+json).then((response) => {
        return response.json();
        });*/
        //<ReactFlowRenderer name={json} />
        setView("CodeFlow");
    }
    console.log(Data);
    console.log(Json);
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
                                    <option value="BillingPROD">Billing PROD</option>
                                    <option value="CAFairfax">CA Fairfax</option>
                                    <option value="CAMooncake">CA Mooncake</option>
                                    <option value="DiagnosticsPROD">Diagnostics PROD</option>
                                    <option value="ExternalPROD">External PROD</option>
                                    <option value="FirstPartyPROD">FirstParty PROD</option>
                                    <option value="Smoke">Smoke</option>
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
                    </div>
                    <div class="footer">
                        <button type="submit" class="btn" onClick={()=>handleSubmit()} >Register</button>
                    </div>
                </div>      
            </>
        );
    }
    else if(View==="CodeFlow")
    {
        return (<>
        <Header />
        <ReactFlowRenderer name={Data}/>
        </>);
    }
}
export default Forms;
