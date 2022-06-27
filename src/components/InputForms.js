import React, {useState} from 'react';
import './Style.css';
import {
    BrowserRouter as Router,Switch,Route,
    Redirect,Link,
} from "react-router-dom";
import Header from './Header.js';

function InputForms() {

    const [DGrepEndpoint, setDGrepEndpoint] = useState("PublicCloud");
    const [MdsEndpoint, setMdsEndpoint] = useState("BlackForest");
    const [Namespace, setNamespace] = useState(null);
    const [EventName,setEventName] = useState(null);
    const [ServiceName,setServiceName] = useState(null);
    const [APIName, setAPIName] = useState(null);
    const [ExternalServiceName, setExternalServiceName] = useState(null);
    const [ExternalAPIName, setExternalAPIName] = useState(null);
    const [ExternalCallType,setExternalCallType] = useState(null);
    const [Certificate,setCertificate] = useState(null);
    const [CertificateRawData,setCertificateRawData] = useState(null);
    const [CertificateName,setCertificateName] = useState(null);

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
        else if(id === "ExternalServiceName"){
            setExternalServiceName(value);
        }
        else if(id === "ExternalAPIName"){
            setExternalAPIName(value);
        }
        else if(id === "ExternalCallType"){
            setExternalCallType(value);
        }
    }
    const handleFileChange = async(e) =>
    {
        setCertificate(e.target.files[0]);
        const get_file_array = (file) => {
            return new Promise((acc, err) => {
                const reader = new FileReader();
                reader.onload = (event) => { acc(event.target.result) };
                reader.onerror = (err)  => { err(err) };
                reader.readAsArrayBuffer(file);
            });
         }
         const temp = await get_file_array(e.target.files[0]);
         setCertificateRawData(temp);
    }
    const handleSubmit  = () => {
        let name="";
        for(let i=0;i<20;i++)
        {
            let x=Math.floor(Math.random()*25);
            name+=String.fromCharCode(65+x);
        }
        var data={};
        data["DGrepEndpoint"]=DGrepEndpoint;
        data["MdsEndpoint"]=MdsEndpoint;
        data["Namespace"]=Namespace;
        data["EventName"]=EventName;
        data["ServiceName"]=ServiceName;
        data["APIName"]=APIName;
        data["ExternalServiceName"]=ExternalServiceName;
        data["ExternalAPIName"]=ExternalAPIName;
        data["ExternalCallType"]=ExternalCallType;
        data["CertificateName"]=name;
        var json=JSON.stringify(data);
        fetch(`/api/hello?name=`+json).then((response) => {
        return response.json();
        });
        
        var base64string = btoa(String.fromCharCode.apply(null, new Uint8Array(CertificateRawData)));
        base64string += ",";
        base64string +=name;
        console.log(base64string);
        fetch("/api/CertBuilder",{
            method: 'post', body: base64string
        }).then(res=>console.log(res))
        .catch(error=>console.log(error));
    }
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
                    <label className="form__label" for="ServiceName">Service Name Column : </label>
                    <input className="form__input" type="text" id="ServiceName" value={ServiceName} onChange = {(e) => handleInputChange(e)} placeholder="Service Name Column"/>
                </div>
                <div className="APIName">
                    <label className="form__label" for="APIName">API Name Column : </label>
                    <input className="form__input" type="text" id="APIName" value={APIName} onChange = {(e) => handleInputChange(e)} placeholder="API Name Column"/>
                </div>
                <div className="ExternalServiceName">
                    <label className="form__label" for="ExternalServiceName">External Service Name Column : </label>
                    <input className="form__input" type="text" id="ExternalServiceName" value={ExternalServiceName} onChange = {(e) => handleInputChange(e)} placeholder="External Service Name Column"/>
                </div>
                <div className="ExternalAPIName">
                    <label className="form__label" for="ExternalAPIName">External API Name Column : </label>
                    <input className="form__input" type="text" id="ExternalAPIName" value={ExternalAPIName} onChange = {(e) => handleInputChange(e)} placeholder="External API Name Column"/>
                </div>
                <div className="ExternalCallType">
                    <label className="form__label" for="ExternalCallType">External Call Type Column : </label>
                    <input className="form__input" type="text" id="ExternalCallType" value={ExternalCallType} onChange = {(e) => handleInputChange(e)} placeholder="External Call Type Column "/>
                </div>
                <div className="Certificate">
                    <label className="form__label" for="Certificate">Certificate : </label>
                    <input className="form__input" type="file" onChange={(e) => handleFileChange(e)} id="Certificate"/>
                </div>
            </div>
            <div class="footer">
                <button type="submit" class="btn btn2" onClick={()=>handleSubmit()} >Register</button>
            </div>
        </div>      
        </>
    )       
}
export default InputForms;