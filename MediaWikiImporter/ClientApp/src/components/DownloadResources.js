import React, { useEffect, useState } from "react";
import { Label } from "reactstrap";
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faHourglass, faCheck, faXmark } from '@fortawesome/free-solid-svg-icons'

function DownloadResourceItem(props) {
    const icons = {
        waiting: faHourglass,
        done: faCheck,
        failed: faXmark
    }
    return (<div >
        <FontAwesomeIcon icon={icons[props.state]} />
        <label >{props.data}</label>
    </div>)
}

export default function DownloadResources(props) {
    const [resources, setResources] = useState(props.resources.map((r) => {
        return { data: r, state: "waiting" };
    }));


    const downloadResource = () => {
        var r = resources.findIndex((e) => e.state == "waiting");
        if (r >= 0) {
            fetch(`api/wiki/${props.resourceType}`, {
                method: "POST",
                body: resources[r].data
            }).then(async (resp) => {
                const newRes = [...resources];
                if (resp.status == 200) {
                    const result = await resp.json()
                    newRes[r].state = result ? "done" : "failed";
                }
                else {
                    newRes[r].state = newRes[r].state == "done" ? "done" : "failed";
                }
                setResources(newRes);
                await downloadResource();
            }).catch(async () => {
                console.log("error");
            });
        }
    }

    useEffect(() => {
        downloadResource();
    }, [resources]);

    if (resources) {
        return (
            <div>
                {resources.length}
                {resources.map((r, index) => {
                    return (<DownloadResourceItem key={`dri_${index}`} state={r.state} data={r.data} />)
                })}
            </div>
        );
    }
    else {
        return <Label>Loading ...</Label>
    }
}