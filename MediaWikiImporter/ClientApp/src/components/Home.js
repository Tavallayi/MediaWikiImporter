import React, { useState } from 'react';
import ListWantedResources from './ListWantedResources';

export default function Home(props) {
    const[s, Sets]= useState('');
    const [data, setData] = useState({ data: [], loadingStatus: "loading" });


    const handleSearch = () => {
       
        fetch(`api/wiki/search?s=${s}`)
            .then(async (resp) => {
                if (resp.status === 200) {
                    const newData = await resp.json();
                    setData({ data: newData.map((d) => ({ title: d, check: true })), loadingStatus: "loaded" });
                } else {
                    setData({ data: [], loadingStatus: "failed" });
                }
            });
    }

    return (
        <div>
            <form>
                <input type='text' id='search' value={s} onChange={(e)=>Sets(e.target.value)}/>
                <input type='button' id='searchBtn' value='Search' onClick={handleSearch} />
            </form>
            <ListWantedResources data={data} title={props.title} resourceType={props.resourceType}/>
        </div>
    );
}
