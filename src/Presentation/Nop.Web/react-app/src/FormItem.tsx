import React, {useState, useCallback, useEffect, memo} from "react";
import {DropdownProps, Option} from "./App"; // Import the type

interface FormItemProps {
    item: DropdownProps;
    style: string;
    isLast?: boolean;
    onPress: (Option : Option) => void;
}

const FormItem: React.FC<FormItemProps> = ({ item, style, isLast ,onPress }) => {
    const [isVisible, setIsVisible] = useState(false);
    const [name, setName] = useState(item.Name);
    
    useEffect(() => {
        setName(() => item.Name);
    },[item.Name]);
    
    const dropdownStyle: React.CSSProperties | undefined = isVisible
        ? {
            position: "absolute",
            inset: "0px auto auto 0px",
            margin: "0px",
            transform: "translate3d(0px, 44.5px, 0px)",
        }
        : undefined;

    // ✅ Memoize toggle function
    const toggleDropdown = useCallback(() => {
        setIsVisible((prev) => !prev);
    }, []);

    // ✅ Memoize option selection function
    const handleOptionClick = useCallback(
        (option : Option) => (e: React.MouseEvent<HTMLAnchorElement>) => {
            e.preventDefault();
            setIsVisible(false);
            setName(option.Name);
            onPress(option);
        },
        [onPress]
    );

    return (
        <div className="col-lg-2 col-md-3 col-sm-6">
            <div className={`dropdown ${isLast ? "" : "border-end-sm"} border-light`}>
                <button className="btn btn-link dropdown-toggle ps-2 ps-sm-3" type="button" onClick={toggleDropdown}>
                    <i className={`${style} me-2`}></i>
                    <span className="dropdown-toggle-label">{name}</span>
                </button>
                <input type="hidden" name={name.toLowerCase()} value={""} />
                <ul className={`dropdown-menu dropdown-menu-dark ${isVisible ? "show" : ""}`.trim()} style={dropdownStyle}>
                    {item.Options.map((option) => (
                        <li key={option.Id}>
                            <a className="dropdown-item" href="#" onClick={handleOptionClick(option)}>
                                <span className="dropdown-item-label">{option.Name}</span>
                            </a>
                        </li>
                    ))}
                </ul>
            </div>
        </div>
    );
};

// ✅ Prevents unnecessary re-renders if props remain the same
export default memo(FormItem);