import React, { useState, useEffect } from 'react'
import DownloadResources from './DownloadResources';

export default function WantedResources(props) {

    const [data, setData] = useState({ data: [], loadingStatus: "loading" });
    const [showDownloadComponenet, setShowDownloadComponenet] = useState(false);


    const fetchData = async () => {
        fetch(`api/wiki/${props.resourceType}`)
            .then(async (resp) => {
                if (resp.status === 200) {
                    const newData = await resp.json();
                    setData({ data: newData.map((d) => ({ title: d, check: true })), loadingStatus: "loaded" });
                } else {
                    setData({ data: [], loadingStatus: "failed" });
                }
            });
    };

    useEffect(() => {
        fetchData();
    }, [])

    const downloadResource = async (event) => {
        setShowDownloadComponenet(true);
        // const response = await fetch(`api/wiki/${props.resourceType}`, {
        //     method: "POST",
        //     body: JSON.stringify(data.data)
        // }).then(() => {
        //     setDownloadSuccessful("Download successfull.")
        //     setData({ data: [], loaded: false });
        //     fetchData();
        // }).catch(() => {
        //     setDownloadSuccessful("Download feiled.")
        // });
    }
    const handleOnBack = async () => {
        setShowDownloadComponenet(false);
        setData({ data: [], loaded: false });
        fetchData();
    }

    if (showDownloadComponenet) {
        console.log("data", data);
        var d = data.data.filter(d => d.check).map(d => d.title);
        console.log("d", d);
        return <DownloadResources resourceType={props.resourceType} resources={d} onBack={handleOnBack} />
    }
    else {
        if (data.loadingStatus === "loaded") {
            return (
                <div>
                    {props.title}<br />
                    Count: {data.data.length}<br />
                    <input type='button' onClick={downloadResource} value="Download" />
                    <div className='row'>
                        {data.data.map((d, i) => (
                            <div key={`d_${i}`} className='col-4'>
                                <input key={`i_${i}`} type="checkbox"
                                    value={d.title}
                                    name={d.title}
                                    checked={d.check}
                                    onChange={(e) => { }}
                                />
                                <label key={`l_${i}`}>{d.title}</label>
                            </div>
                        ))}
                    </div>

                </div>);
        }
        else if (data.loadingStatus === "loading") {
            return (<div>
                Loading {props.title} ...
            </div>);
        }
        else {
            return (<div>
                Loading {props.title} failed. Please refresh page.
            </div>);
        }
    }
}