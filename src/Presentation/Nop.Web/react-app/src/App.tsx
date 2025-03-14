import React, {useCallback, useEffect, useState} from "react";
import FormItem from "./FormItem.tsx";
const API_URL = import.meta.env.VITE_API_URL as string;
export interface Option {
    Name: string; 
    SeoName?: string; 
    Id: number; 
}
export interface DropdownProps{
    Name: string;
    Icon: string;
    Options: Option[];
}

const SearchForm: React.FC<{ dropdowns: DropdownProps[] }> = (props) => {
    const [activeTab, setActiveTab] = useState("New");
    const [seoName, setSeoName] = useState<string | undefined>();
    const [bodyTypeSelection, setBodyTypeSelection] = useState<number | undefined>();
    const [locationTypeSelection, setLocationTypeSelection] = useState<number>()
    const [makeId, setMakeId] = useState<number>();
    const [modelId, setModelId] = useState<number>();
    const [make] = useState<DropdownProps>(props.dropdowns[0]);
    //const [isDropdownVisible, setIsDropdownVisible] = useState(false);
    const [model, setModel] = useState<DropdownProps>(props.dropdowns[1]);
    const [bodyType,setBodyType] = useState<DropdownProps>(props.dropdowns[2]);
    const [location,setLocation] = useState<DropdownProps>(props.dropdowns[3]);
    
    const handleClick = (tab: string) => {
        setActiveTab(tab);
    };

    const specificationRequest = (categoryId: number, commandType: "BODY_TYPE" | "LOCATIONS") => ({
        categoryId,
        commandType,
    });

    useEffect(() => {
        (async () => {
            if (makeId === undefined) return; // ✅ Prevent API call when make is null
            try {
                const response = await fetch(`${API_URL}/SellCar/ChildrenCategories`, {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json",
                    },
                    body: JSON.stringify(makeId),
                });

                const responseBodyType = await fetch(`${API_URL}/SellCar/GetSpecificationsByCategory`, {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json",
                    },
                    body: JSON.stringify(specificationRequest(makeId,"BODY_TYPE")),
                });

                const responseLocationType = await fetch(`${API_URL}/SellCar/GetSpecificationsByCategory`, {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json",
                    },
                    body: JSON.stringify(specificationRequest(makeId,"LOCATIONS")),
                });

                if (!responseBodyType.ok) {
                    throw new Error(`HTTP error! Status: ${response.status}`);
                }

                if (!response.ok) {
                    throw new Error(`HTTP error! Status: ${response.status}`);
                }
                const result: { Text: string; Value: string, SeoName: string }[] = await response.json(); // Ensure correct API response type
                const resultBodyType: { Id : number, Name : string }[] = await responseBodyType.json(); // Ensure correct API response type
                const resultLocationType: { Id : number, Name : string }[] = await responseLocationType.json(); // Ensure correct API response type

                // ✅ Convert API response to match `Options` type
                const options: Option[] = result.map((item) => ({
                    Name: item.Text, // Use API's "Text" as "Name"
                    SeoName: item.SeoName,
                    Id: Number(item.Value) // Ensure `Id` is a number
                }));

                // ✅ Convert API response to match `Options` type
                const bodyType: Option[] = resultBodyType.map((item) => ({
                    Name: item.Name, 
                    Id: item.Id
                }));

                const locationType: Option[] = resultLocationType.map((item) => ({
                    Name: item.Name,
                    Id: item.Id
                }));

                setLocation((prevState) => ({
                    ...prevState,
                    Name: "Location",
                    Options: locationType,
                }))
                
                setBodyType((prevState) => ({
                    ...prevState,
                    Name: "Body type",
                    Options: bodyType,
                }))

                // ✅ Ensure the function returns a valid `DropdownProps[]`
                setModel((prevModel) => ({
                    ...prevModel, // Spread existing properties (Name, Icon)
                    Name: "Model",
                    Options: options // Update only Options
                }));
            } catch (err) {
                //setError(err.message);
                console.error("Fetch Error:", err);
            }
        })();
    }, [makeId]);

    useEffect(() => {
        if (modelId === undefined) return;
        (async () => {
            const responseBodyType = await fetch(`${API_URL}/SellCar/GetSpecificationsByCategory`, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                },
                body: JSON.stringify(specificationRequest(modelId,"BODY_TYPE")),
            });
            const resultBodyType: { Id : number, Name : string }[] = await responseBodyType.json(); // Ensure correct API response type
            // ✅ Convert API response to match `Options` type
            const bodyType: Option[] = resultBodyType.map((item) => ({
                Name: item.Name,
                Id: item.Id
            }));
            setBodyType((prevState) => ({
                ...prevState,
                Name: "Body type",
                Options: bodyType,
            }))
        })();
        
        
    }, [modelId]);

    const handleMakeSelect = useCallback((option: Option) => {
        setSeoName(option.SeoName)
        setMakeId(option.Id);
    }, []);
    const handleModelSelect = useCallback((option: Option) => {
        setSeoName(option.SeoName)
        setModelId(option.Id)
        setModel((prevModel) => ({
            ...prevModel,
            Name: option.Name,
        }));
    }, []);
    
    return (
        <>
            {/* Tabs */}
            <ul className="nav nav-tabs nav-tabs-light mb-4">
                <li className="nav-item">
                    <a className={`nav-link${activeTab === "New" ? " active" : ""}`} href="#" onClick={(e) => {
                        e.preventDefault();
                        handleClick("New");
                    }}>New</a>
                </li>
                <li className="nav-item">
                    <a className={`nav-link${activeTab === "Used" ? " active" : ""}`} href="#" onClick={(e) => {
                        e.preventDefault();
                        handleClick("Used");
                    }}>Used</a>
                </li>
            </ul>

            {/* Form group */}
            <form className="form-group form-group-light d-block">
                <div className="row g-0 ms-lg-n2">
                    {/* Keywords Input */}
                    <div className="col-lg-2">
                        <div className="input-group border-end-lg border-light">
                        <span className="input-group-text text-muted ps-2 ps-sm-3">
                            <i className="fi-search"></i>
                        </span>
                            <input className="form-control" placeholder="Keywords..." type="text" name="keywords"/>
                        </div>
                    </div>
                    <hr className="hr-light d-lg-none my-2"/>
                    {/* Dynamic Dropdowns */}
                    <FormItem item={make} style={"fi-list"} onPress={handleMakeSelect}/>
                    <FormItem item={model} style={"fi-list"} onPress={handleModelSelect}/>
                    <FormItem item={bodyType} style={"fi-car fs-lg"} onPress={useCallback((option) => {
                        setBodyType((prevState) => ({
                            ...prevState,
                            Name: option.Name,
                        }))
                        setBodyTypeSelection(option.Id);
                    }, [])}/>
                    <FormItem item={location} style={"fi-map-pin"} isLast={true} onPress={useCallback((option) => {
                        setLocation((prevState) => ({
                            ...prevState,
                            Name: option.Name,
                        }))
                        setLocationTypeSelection(option.Id);
                    }, [])}/>
                    
                    {/* Search Button */}
                    <div className="col-lg-2">
                        <button className="btn btn-primary w-100" type="button" onClick={() => {
                            let link = `https://localhost:5001/${seoName}`;
                            let specsParams: number[] = [];
                            
                            if (bodyTypeSelection !== undefined) {
                                specsParams.push(bodyTypeSelection);
                            }
                            
                            if (locationTypeSelection !== undefined) {
                                specsParams.push(locationTypeSelection);
                            }
                            
                            if (specsParams.length > 0) {
                                link += `?specs=${encodeURIComponent(specsParams.join(","))}`;
                            }
                            
                            window.location.href = link;
                        }}>
                            Search
                        </button>
                    </div>
                </div>
            </form>
        </>
    );
};

export default SearchForm;
