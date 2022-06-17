import React, {useState} from 'react';
import './Style.css'
function InputForms() {

    const [DGrepEndpoint, setDGrepEndpoint] = useState("PublicCloud");
    const [MdsEndpoint, setMdsEndpoint] = useState("BillingPROD");
    const [Namespace, setNamespace] = useState(null);
    const [EventName,setEventName] = useState(null);
    const [ColumnName,setColumnName] = useState(null);
    const [ServiceFormat, setServiceFormat] = useState(null);
    const [SystemIdColumnName, setSystemIdColumnName] = useState(null);
    const [ExternalIdColumnName, setExternalIdColumnName] = useState(null);
    const [RepititiveTaskId,setRepititiveTaskId] = useState(null);
    const [TimeStampWindow,setTimeStampWindow] = useState(null);
    const [Certificate,setCertificate] = useState(null);

    const handleInputChange = (e) => {
        const {id , value} = e.target;
        if(id === "Namespace"){
            setNamespace(value);
        }
        else if(id === "EventName"){
            setEventName(value);
        }
        else if(id === "ColumnName"){
            setColumnName(value);
        }
        else if(id === "ServiceFormat"){
            setServiceFormat(value);
        }
        else if(id === "SystemIdColumnName"){
            setSystemIdColumnName(value);
        }
        else if(id === "ExternalIdColumnName"){
            setExternalIdColumnName(value);
        }
        else if(id === "RepititiveTaskId"){
            setRepititiveTaskId(value);
        }
        else if(id === "TimeStampWindow"){
            setTimeStampWindow(value);
        }
        else if(id === "DGrepEndpoint"){
            setDGrepEndpoint(value);
        }
        else if(id === "MdsEndpoint"){
            setMdsEndpoint(value);
        }
    }
    const handleFileChange = (e) =>
    {
        setCertificate(e.target.files[0]);
    }

    const handleSubmit  = () => {
        var data={};
        data["DGrepEndpoint"]=DGrepEndpoint;
        data["Namespace"]=Namespace;
        data["MdsEndpoint"]=MdsEndpoint;
        data["TimeStampWindow"]=TimeStampWindow;
        data["RepititiveTaskId"]=RepititiveTaskId;
        data["EventName"]=EventName;
        data["ColumnName"]=ColumnName;
        data["ServiceFormat"]=ServiceFormat;
        data["SystemIdColumnName"]=SystemIdColumnName;
        data["Namespace"]=Namespace;
        data["ExternalIdColumnName"]=ExternalIdColumnName;
        data["Certificate"]=Certificate;
        data["CertificateName"]=Certificate.name;    
        var json=JSON.stringify(data);
       var v =  fetch("/api/hello?name="+data,{
            method: 'GET'
        });/*
        fetch("https://api20220617072943.azurewebsites.net/api/hello?name="+data,{
            method: 'GET'
        });*/

        fetch('/api/hello')
  .then((response) => {
    return response.json();
  })
  .then((myJson) => {
    console.log(myJson);
  });

        console.log(v);
        console.log(json);
        console.log(DGrepEndpoint,MdsEndpoint,Namespace,EventName,ColumnName,ServiceFormat,SystemIdColumnName,ExternalIdColumnName,RepititiveTaskId,TimeStampWindow);
    }

    return(
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
              <div className="ColumnName">
                  <label className="form__label" for="ColumnName">Column Name : </label>
                  <input className="form__input" type="text" id="ColumnName" value={ColumnName} onChange = {(e) => handleInputChange(e)} placeholder="Column Name"/>
              </div>
              <div className="ServiceFormat">
                  <label className="form__label" for="ServiceFormat">Service Format : </label>
                  <input className="form__input" type="text" id="ServiceFormat" value={ServiceFormat} onChange = {(e) => handleInputChange(e)} placeholder="Service Format"/>
              </div>
              <div className="SystemIdColumnName">
                  <label className="form__label" for="SystemIdColumnName">System Id Column Name : </label>
                  <input className="form__input" type="text" id="SystemIdColumnName" value={SystemIdColumnName} onChange = {(e) => handleInputChange(e)} placeholder="System ID Column Name"/>
              </div>
              <div className="ExternalIdColumnName">
                  <label className="form__label" for="ExternalIdColumnName">External Id Column Name : </label>
                  <input className="form__input" type="text" id="ExternalIdColumnName" value={ExternalIdColumnName} onChange = {(e) => handleInputChange(e)} placeholder="External ID Column Name"/>
              </div>
              <div className="RepititiveTaskId">
                  <label className="form__label" for="RepititiveTaskId">Repititive Task ID : </label>
                  <input className="form__input" type="text" id="RepititiveTaskId" value={RepititiveTaskId} onChange = {(e) => handleInputChange(e)} placeholder="Repititive Task ID "/>
              </div>
              <div className="TimeStampWindow">
                  <label className="form__label" for="TimeStampWindow">Timestamp Window : </label>
                  <input className="form__input" type="datetime-local" value={TimeStampWindow} onChange = {(e) => handleInputChange(e)} id="TimeStampWindow"/>
              </div>
              <div className="Certificate">
                  <label className="form__label" for="Certificate">Certificate : </label>
                  <input className="form__input" type="file" onChange={(e) => handleFileChange(e)} id="Certificate"/>
              </div>
          </div>
          <div class="footer">
              <button type="submit" class="btn" onClick={()=>handleSubmit()} >Register</button>
          </div>
      </div>      
    )       
}
export default InputForms;