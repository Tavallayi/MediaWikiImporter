import React, { useState, useEffect } from 'react'
import ListWantedResources from './ListWantedResources';

export default function WantedResources(props) {

    const [data, setData] = useState({ data: [], loadingStatus: "loading" });


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

    const handleOnBack = async () => {
        setData({ data: [], loaded: false });
        fetchData();
    }
    return (<ListWantedResources data={data} onBack={handleOnBack} title={props.title} resourceType={props.resourceType}/>)
}