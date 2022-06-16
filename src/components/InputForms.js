import React, {useState} from 'react';
import './Style.css'
function InputForms() {
    return(
      <div className="form">
          <div className="form-body">
              <div className="DGrepEndpoint">
                  <label className="form__label" for="DGrepEndpoint">DGrepEndpoint : </label>
                  <select name="DGrepEndpoint" className="form__input" id="DGrepEndpoint">
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
                    <select name="MdsEndpoint" id="MdsEndpoint" className="form__input">
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
                  <label className="form__label" for="Namespace">Email : </label>
                  <input  type="text" id="Namespace" className="form__input" placeholder="Namespace"/>
              </div>
              <div className="EventName">
                  <label className="form__label" for="EventName">Event Name : </label>
                  <input className="form__input" type="text"  id="EventName" placeholder="Event Name"/>
              </div>
              <div className="ColumnName">
                  <label className="form__label" for="ColumnName">Column Name : </label>
                  <input className="form__input" type="text" id="ColumnName" placeholder="Column Name"/>
              </div>
              <div className="ServiceFormat">
                  <label className="form__label" for="ServiceFormat">Service Format : </label>
                  <input className="form__input" type="text" id="ServiceFormat" placeholder="Service Format"/>
              </div>
              <div className="SystemIdColumnName">
                  <label className="form__label" for="SystemIdColumnName">System Id Column Name : </label>
                  <input className="form__input" type="text" id="SystemIdColumnName" placeholder="System ID Column Name"/>
              </div>
              <div className="ExternalIdColumnName">
                  <label className="form__label" for="ExternalIdColumnName">External Id Column Name : </label>
                  <input className="form__input" type="text" id="ExternalIdColumnName" placeholder="External ID Column Name"/>
              </div>
              <div className="RepititiveTaskId">
                  <label className="form__label" for="RepititiveTaskId">Repititive Task ID : </label>
                  <input className="form__input" type="text" id="RepititiveTaskId" placeholder="Repititive Task ID "/>
              </div>
              <div className="TimeStampWindow">
                  <label className="form__label" for="TimestampWindow">Timestamp Window : </label>
                  <input className="form__input" type="datetime-local" id="TimestampWindow"/>
              </div>
              <div className="Certificate">
                  <label className="form__label" for="Certificate">Certificate : </label>
                  <input className="form__input" type="file" id="Certificate"/>
              </div>
          </div>
          <div class="footer">
              <button type="submit" class="btn">Register</button>
          </div>
      </div>      
    )       
}
export default InputForms;